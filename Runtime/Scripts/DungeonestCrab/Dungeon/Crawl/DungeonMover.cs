using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KH;
using KH.Input;
using KH.Audio;
using Ratferences;
using KH.Extensions;
using Sirenix.OdinInspector;
using static DungeonestCrab.Dungeon.Crawl.DungeonGrid;

namespace DungeonestCrab.Dungeon.Crawl {
    public class DungeonMover : DungeonEntity, DungeonGrid.GridObject {
        [SerializeField] Controller Controller;
        [SerializeField] AudioEvent FootstepSound;
        [SerializeField] AudioEvent WallMoveSound;
        [SerializeField] BoolReference PlayerMovementInputAllowed;
        [SerializeField] BoolReference PlayerInitiatedInteraction;
        [SerializeField] bool InteractOnBump;
        [Tooltip("Time to wait before allowing another interaction. This is primarily to avoid the interact close input beginning another interaction.")]
        [SerializeField] float InteractCooldown = 0.2f;
        [SerializeField] DungeonMovementType MovementType = DungeonMovementType.Timed;
        [ShowIf("MovementType", DungeonMovementType.Instant)] [SerializeField] InstantMovementConfig InstantMovement;
        [ShowIf("MovementType", DungeonMovementType.Timed)] [SerializeField] TimedMovementConfig TimedMovement;

        Action _bufferedAction;
        private Dictionary<Action, SingleInputMediator> _actions = new Dictionary<Action, SingleInputMediator>();

        private TimeCheck _timeCheck;

        enum DungeonMovementType {
            Instant,
            Timed
        }

        public enum Action {
            None,
            MoveLeft,
            MoveForward,
            MoveRight,
            MoveBack,
            TurnLeft,
            TurnRight,
            Wait,
            Interact,
        }

        public enum ActionType {
            Move,
            Turn,
            Interact,
            Other
        }

        private void Awake() {
            if (PlayerMovementInputAllowed != null) PlayerMovementInputAllowed.Value = true;
            this.transform.localRotation = Quaternion.Euler(Vector3.up * _angle);
            this.transform.localPosition = new Vector3(Mathf.Round(this.transform.localPosition.x), 0, Mathf.Round(this.transform.localPosition.z));
            DungeonGrid.INSTANCE.RegisterPlayer(this);
            _timeCheck = new TimeCheck();
            _actions[Action.Interact] = Controller.Interact;
            _actions[Action.MoveForward] = Controller.MoveForward;
            _actions[Action.MoveBack] = Controller.MoveBack;
            _actions[Action.MoveLeft] = Controller.MoveLeft;
            _actions[Action.MoveRight] = Controller.MoveRight;
            _actions[Action.TurnLeft] = Controller.TurnLeft;
            _actions[Action.TurnRight] = Controller.TurnRight;

            // Both are initialized as theoretically they could be swapped mid-game.
            TimedMovement.OnInit();
            InstantMovement.OnInit();
        }

        public void TeleportToGridObject(DungeonGrid.GridObject obj, float angle) {
            TeleportToGrid(obj.GridItemInfo().GridPosition, angle);
        }

        public void TeleportToGrid(Vector2 gridPosition, float angle) {
            Teleport(DungeonGrid.INSTANCE.InfoForGridPosition(gridPosition).WorldPosition, angle);
        }

        public void Teleport(Vector3 pos, float angle) {
            this.transform.position = new Vector3(pos.x, this.transform.position.y, pos.z);
            _angle = angle;
            this.transform.localRotation = Quaternion.Euler(Vector3.up * _angle);
        }

        public void Teleport(Vector2 pos, float angle) {
            Teleport(pos.ToVectorX0Y(), angle);
        }

        private float DirToAngle(DungeonGrid.Node.Dir dir) {
            return dir switch {
                DungeonGrid.Node.Dir.North => 0,
                DungeonGrid.Node.Dir.East => 90,
                DungeonGrid.Node.Dir.South => 180,
                DungeonGrid.Node.Dir.West => 270,
                _ => 0,
            };
        }

        public void Teleport(DungeonFloor pos, DungeonGrid.Node.Dir dir) {
            TeleportToGrid(pos.GridItemInfo().GridPosition, DirToAngle(dir));
        }


        public void TryMoveLeft() {
            TryDoAction(Action.MoveLeft);
        }

        public void TryMoveRight() {
            TryDoAction(Action.MoveRight);
        }

        public void TryMoveForward() {
            TryDoAction(Action.MoveForward);
        }

        public void TryMoveBack() {
            TryDoAction(Action.MoveBack);
        }

        public void TryTurnLeft() {
            TryDoAction(Action.TurnLeft);
        }

        public void TryTurnRight() {
            TryDoAction(Action.TurnRight);
        }

        public void TryWait() {
            TryDoAction(Action.Wait);
        }

        public bool TryDoAction(Action action) {
            if (action == Action.None) return false;
            // Can't do action on first frame to prevent
            // holdover from scene transition.
            if (!_timeCheck.HasTimeElapsed(0)) return false;
            if (!MovementInputAllowed()) {
                return false;
            }
            if (!MovementAllowed) {
                _bufferedAction = action;
                return false;
            }

            HandleTurn(action);
            return true;
        }

        protected override void OnDidRotate() {
            MaybePlay(FootstepSound);
        }

        protected override void OnWillMove(Vector3 destination, float duration) {
            if (MovementConfig.InstantlyUpdatesPosition) _positionOverride = destination;
        }

        protected override void OnDidMove() {
            _positionOverride = null;
            MaybePlay(FootstepSound);
        }

        protected override void OnWallBump() {
            MaybePlay(WallMoveSound);
        }

        private void MaybePlay(AudioEvent aevent) {
            if (aevent != null) aevent.PlayOneShot();
        }

        ActionType TypeForAction(Action action) {
            return action switch {
                Action.MoveLeft or Action.MoveForward or Action.MoveRight or Action.MoveBack => ActionType.Move,
                Action.TurnLeft or Action.TurnRight => ActionType.Turn,
                Action.Interact => ActionType.Interact,
                _ => ActionType.Other,
            };
        }

        void HandleTurn(Action action) {
            if (action == Action.Interact) {
                if (!_timeCheck.HasTimeElapsed(InteractCooldown)) return;
                TryInteract();
                return;
            }
            float animTime = MovementConfig.TimeForAction(action);

            TurnAction turnAction = action switch {
                Action.MoveLeft => TurnAction.MoveLeft,
                Action.MoveForward => TurnAction.MoveForward,
                Action.MoveRight => TurnAction.MoveRight,
                Action.MoveBack => TurnAction.MoveBack,
                Action.TurnLeft => TurnAction.TurnLeft,
                Action.TurnRight => TurnAction.TurnRight,
                Action.Wait => TurnAction.DoNothing,
                // these shouldn't happen
                Action.None => TurnAction.DoNothing,
                // I shouldn't need a default (since this is exhaustive)
                // but it's yelling at me regardless so it's all noise anyway.
                _ => TurnAction.DoNothing,
            };
            
            StartCoroutine(DoTurn(turnAction, animTime, InteractOnBump));
        }

        IEnumerator DoTurn(TurnAction turnAction, float animTime, bool interactOnBump) {
            _inMovement = true;
            MoveAttempt attempt = DungeonGrid.INSTANCE.GetMoveAttempt(this, turnAction);
            yield return DungeonGrid.INSTANCE.DoSingleMove(this, turnAction, MovementConfig, interactOnBump);
            _inMovement = false;
        }

        private void TryInteract() {
            DungeonGrid.INSTANCE.HandleInteractions(this);
        }

        public IEnumerator HandleInteract(DungeonInteractable interactable) {
            // If it's resolving an interaction, cancel the buffer.
            _bufferedAction = Action.None;
            yield return TryInteract(interactable);
        }

        /// <summary>
        /// Rotates to face interactable and interacts, waiting for interaction to complete.
        /// </summary>
        private IEnumerator TryInteract(DungeonInteractable interactable) {
            if (interactable == null) yield break;
            else {
                Vector2 dir = interactable.GridItemInfo().GridPosition - GridItemInfo().GridPosition;
                if (interactable.FaceOnInteract) {
                    for (int i = 0; i < 2; i++) {
                        Vector2 normalizedDir = NormalizedRotation(dir);

                        int rotAmt = 0;
                        if (normalizedDir.x == 1) rotAmt = -90;
                        else if (normalizedDir.x == -1) rotAmt = 90;
                        else if (normalizedDir.y == -1) rotAmt = 90;
                        else break;

                        if (rotAmt == 0) {
                            break;
                        } else {
                            yield return DoRotate(_angle + rotAmt, MovementConfig.TimeForAction(Action.TurnLeft));
                        }
                    }
                }
                bool moveTowardsEnemy = (_lastMove.x > 0 && dir.x > 0 || _lastMove.y > 0 && dir.y > 0 || _lastMove.x < 0 && dir.x < 0 || _lastMove.y < 0 && dir.y < 0);
                if (PlayerInitiatedInteraction != null) PlayerInitiatedInteraction.Value = moveTowardsEnemy;
                yield return interactable.HandleInteraction();
            }
        }

        private bool MovementInputAllowed() {
            return PlayerMovementInputAllowed == null || PlayerMovementInputAllowed.Value == true;
        }

        public void OnReadyForInput() {
            _timeCheck.UpdateTime();
            CheckAndDoMove();
        }

        private void HandleQueuedAction() {
            if (_bufferedAction != Action.None) {
                if (TryDoAction(_bufferedAction)) {
                    MovementConfig.ActionTriggered(TypeForAction(_bufferedAction), _bufferedAction);
                    _bufferedAction = Action.None;
                }
            }
        }

        private void CheckAndDoMove() {
            if (!MovementInputAllowed() || Time.timeScale == 0) return;

            //Down checks should be processed on press, even
            // if the player is already moving.
            Action[] actionOrder = new Action[] {
                Action.MoveLeft,
                Action.MoveRight,
                Action.MoveForward,
                Action.MoveBack,
                Action.TurnLeft,
                Action.TurnRight,
                Action.Interact
            };

            // Queue up button presses.
            foreach (Action action in actionOrder) {
                if (MovementConfig.AllowEnqueue(TypeForAction(action), action) && _actions[action].InputJustDown()) {
                    _bufferedAction = action;
                    break;
                }
            }
            if (MovementAllowed) {
                foreach (Action action in actionOrder) {
                    if (_actions[action].InputJustDown()) {
                        _bufferedAction = action;
                        break;
                    }
                    if (MovementConfig.AllowAutoRepeat(TypeForAction(action), action) && _actions[action].InputDown()) {
                        _bufferedAction = action;
                        break;
                    }
                }

                HandleQueuedAction();
            }
        }

        private void Update() {
            CheckAndDoMove();
        }

        IMovementConfig MovementConfig => MovementType == DungeonMovementType.Instant ? InstantMovement : TimedMovement;

        public interface IMovementConfig {
            void OnInit();
            float TimeForAction(Action action);
            bool AllowEnqueue(ActionType type, Action action);
            bool AllowAutoRepeat(ActionType type, Action action);
            void ActionTriggered(ActionType type, Action action);
            float BumpPercentage { get; }
            bool InstantlyUpdatesPosition { get; }
        }

        [System.Serializable]
        public class InstantMovementConfig : IMovementConfig {
            [SerializeField] bool RetriggerAfterDelay;
            [ShowIf("RetriggerAfterDelay")] [SerializeField] float RetriggerDelay = 0.25f;
            [Tooltip("If set, retrigger will only occur if this key is held down. If not set, it will always happen.")]
            [ShowIf("RetriggerAfterDelay")] [SerializeField] SingleInputMediator RetriggerInput;
            [ShowIf("RetriggerAfterDelay")] [SerializeField] bool AutoRepeatMoves = true;
            [ShowIf("RetriggerAfterDelay")] [SerializeField] bool AutoRepeatTurns = false;
            private TimeCheck _check;

            public float BumpPercentage => 0;

            public bool InstantlyUpdatesPosition => true;

            public void OnInit() {
                _check = new TimeCheck();
            }

            void IMovementConfig.ActionTriggered(ActionType type, Action action) {
                _check.UpdateTime();
            }

            bool IMovementConfig.AllowEnqueue(ActionType type, Action action) {
                return false;
            }

            bool IMovementConfig.AllowAutoRepeat(ActionType type, Action action) {
                if (!_check.HasTimeElapsed(RetriggerDelay) || (RetriggerInput != null && !RetriggerInput.InputDown())) {
                    return false;
                }
                if (type == ActionType.Move && AutoRepeatMoves) return true;
                if (type == ActionType.Turn && AutoRepeatTurns) return true;
                else return false;
            }

            float IMovementConfig.TimeForAction(Action action) {
                return 0;
            }
        }

        [System.Serializable]
        public class TimedMovementConfig : IMovementConfig {
            [SerializeField] bool EnqueueMoves = true;
            [SerializeField] bool EnqueueTurns = true;
            [SerializeField] bool AutoRepeatMoves = true;
            [SerializeField] bool AutoRepeatTurns = true;
            [SerializeField] bool UniformMovementTime = true;
            [SerializeField] float RotationTime = 0.3f;
            [ShowIf("UniformMovementTime")] [SerializeField] float MoveTime = 0.5f;
            [HideIf("UniformMovementTime")] [SerializeField] float MoveForwardTime = 0.4f;
            [HideIf("UniformMovementTime")] [SerializeField] float StrafeTime = 0.45f;
            [HideIf("UniformMovementTime")] [SerializeField] float BackstepTime = 0.5f;
            [SerializeField] float WaitTime = 0.3f;
            [SerializeField] float BumpPercentage = 0.3f;
            [SerializeField] bool InstantlyUpdatesPosition = true;

            bool IMovementConfig.InstantlyUpdatesPosition => InstantlyUpdatesPosition;

            float IMovementConfig.BumpPercentage => BumpPercentage;

            public void OnInit() {}
            void IMovementConfig.ActionTriggered(ActionType type, Action action) {}

            bool IMovementConfig.AllowAutoRepeat(ActionType type, Action action) {
                if (type == ActionType.Move && AutoRepeatMoves) return true;
                if (type == ActionType.Turn && AutoRepeatTurns) return true;
                else return false;
            }

            bool IMovementConfig.AllowEnqueue(ActionType type, Action action) {
                if (type == ActionType.Move && EnqueueMoves) return true;
                if (type == ActionType.Turn && EnqueueTurns) return true;
                else return false;
            }

            float IMovementConfig.TimeForAction(Action action) {
                return action switch {
                    Action.Wait => WaitTime,
                    Action.TurnLeft or Action.TurnRight => RotationTime,
                    Action.MoveLeft => UniformMovementTime ? MoveTime : StrafeTime,
                    Action.MoveForward => UniformMovementTime ? MoveTime : MoveForwardTime,
                    Action.MoveRight => UniformMovementTime ? MoveTime : StrafeTime,
                    Action.MoveBack => UniformMovementTime ? MoveTime : BackstepTime,
                    _ => 0,
                };
            }
        }
    }
}