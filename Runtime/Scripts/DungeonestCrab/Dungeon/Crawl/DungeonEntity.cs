using KH;
using KH.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DungeonestCrab.Dungeon.Crawl {
    public class DungeonEntity : MonoBehaviour, DungeonGrid.GridObject, DungeonGrid.Resettable {

        public enum TurnAction {
            MoveForward,
            MoveLeft,
            MoveRight,
            MoveBack,
            MoveNorth,
            MoveWest,
            MoveEast,
            MoveSouth,
            TurnLeft,
            TurnRight,
            DoNothing
        }

        public float Angle {
            get => _angle;
        }

        public bool MovementAllowed {
            get => DungeonGrid.INSTANCE.MovementAllowed && !_inMovement;
        }

        public UnityEvent OnReset;
        public UnityEvent OnAfterMove;

        public delegate void BeforeMove();
        public BeforeMove OnBeforeMove;

        protected float _angle;
        protected Vector2Int _lastMove;
        protected bool _inMovement = false;
        protected Vector3? _positionOverride;

        public DungeonGrid.GridItemInfo GridItemInfo() {
            Vector3 pos = transform.position;
            if (_positionOverride.HasValue) pos = _positionOverride.Value;
            return DungeonGrid.INSTANCE.InfoForLocation(pos);
        }

        private Vector2Int ActionToMoveDir(TurnAction action) {
            return action switch {
                TurnAction.MoveForward => NormalizedMoveDirection(new Vector2Int(0, 1)),
                TurnAction.MoveBack => NormalizedMoveDirection(new Vector2Int(0, -1)),
                TurnAction.MoveLeft => NormalizedMoveDirection(new Vector2Int(-1, 0)),
                TurnAction.MoveRight => NormalizedMoveDirection(new Vector2Int(1, 0)),
                TurnAction.MoveNorth => new Vector2Int(0, 1),
                TurnAction.MoveSouth => new Vector2Int(0, -1),
                TurnAction.MoveWest => new Vector2Int(-1, 0),
                TurnAction.MoveEast => new Vector2Int(1, 0),
                _ => Vector2Int.zero,
            };
        }

        public virtual TurnAction GetTurnAction() {
            if (TryGetComponent<DungeonEntityMover>(out var mover)) {
                return mover.GetTurnAction();
            }
            return TurnAction.DoNothing;
        }

        public Vector2Int RealMoveDir(TurnAction action) {
            return ActionToMoveDir(action);
        }

        private DungeonMover.Action ActionForTurnAction(TurnAction action) {
            return action switch {
                TurnAction.MoveForward => DungeonMover.Action.MoveForward,
                TurnAction.MoveLeft => DungeonMover.Action.MoveLeft,
                TurnAction.MoveRight => DungeonMover.Action.MoveRight,
                TurnAction.MoveBack => DungeonMover.Action.MoveBack,
                TurnAction.TurnLeft => DungeonMover.Action.TurnLeft,
                TurnAction.TurnRight => DungeonMover.Action.TurnRight,
                // No context on actual move direction, but in this case it's *probably* a mob doing the movement,
                // so it should go with a default speed unless they care enough to pass that in.
                TurnAction.MoveNorth or TurnAction.MoveWest or TurnAction.MoveEast or TurnAction.MoveSouth => DungeonMover.Action.MoveForward,
                _ => DungeonMover.Action.None,
            };
        }

        public IEnumerator DoTurnAction(TurnAction action, DungeonGrid.MoveAttempt attempt, DungeonMover.IMovementConfig movementConfig) {
            float duration = movementConfig.TimeForAction(ActionForTurnAction(action));
            switch (action) {
                case TurnAction.TurnLeft:
                    _lastMove = Vector2Int.zero;
                    yield return DoRotate(_angle - 90, duration);
                    break;
                case TurnAction.TurnRight:
                    _lastMove = Vector2Int.zero;
                    yield return DoRotate(_angle + 90, duration);
                    break;
                case TurnAction.MoveForward:
                case TurnAction.MoveBack:
                case TurnAction.MoveLeft:
                case TurnAction.MoveRight:
                case TurnAction.MoveNorth:
                case TurnAction.MoveSouth:
                case TurnAction.MoveWest:
                case TurnAction.MoveEast:
                    Vector2Int dir = ActionToMoveDir(action);
                    _lastMove = dir;
                    yield return DoMove(dir, attempt.isBump, attempt.isWallBump, duration, movementConfig);
                    break;
                case TurnAction.DoNothing:
                default:
                    // Other moving items will do the delay. Worst case, it'll be
                    // the DungeonGrid.
                    yield break;
            }
        }

        public bool CanDoMove(TurnAction action) {
            return DungeonGrid.INSTANCE.CanDoMove(this, action);
        }

        private IEnumerator DoMove(Vector2Int dir, bool bump, bool wallBump, float duration, DungeonMover.IMovementConfig movementConfig) {
            _lastMove = dir;
            if (dir.sqrMagnitude < 1) {
                Debug.Log("Tried to move in no direction");
                yield break;
            }

            Vector3 offset = DungeonGrid.INSTANCE.CanonicalOffset(dir.ToVectorX0Y()).ToVectorX0Y();
            Vector3 dest = this.transform.position + offset;

            if (!bump) {
                OnBeforeMove?.Invoke();
                yield return MoveComputed(transform.position + offset, duration);
                OnAfterMove?.Invoke();
            } else {
                yield return MoveBump(transform.position + 2 * movementConfig.BumpPercentage * offset, wallBump, duration);
            }
        }

        protected Vector2Int NormalizedMoveDirection(Vector2 dir) {
            Vector2 rotated = Quaternion.AngleAxis(-_angle, Vector3.forward) * dir;

            Vector2Int normalized = Vector2Int.zero;
            if (rotated.x > 0.1) normalized.x = 1;
            else if (rotated.x < -0.1) normalized.x = -1;
            else if (rotated.y > 0.1) normalized.y = 1;
            else if (rotated.y < -0.1) normalized.y = -1;

            return normalized;
        }

        IEnumerator MoveComputed(Vector3 dest, float duration) {
            Vector3 start = this.transform.position;

            OnWillMove(dest, duration);
            yield return EZTween.DoPercentAction((float newPercent) => {
                this.transform.position = Vector3.Lerp(start, dest, newPercent);
            }, duration, EZTween.Curve.CubicEaseInOut);
            OnDidMove();
        }

        IEnumerator MoveBump(Vector3 dest, bool wallBump, float duration) {
            Vector3 start = this.transform.position;

            bool handledWallBump = false;

            OnWillMove(dest, duration);
            yield return EZTween.DoPercentAction((float newPercent) => {
                if (newPercent < 0.5f) {
                    this.transform.position = Vector3.Lerp(start, dest, newPercent);
                } else {
                    if (!handledWallBump && wallBump) {
                        OnWallBump();
                        handledWallBump = true;
                    }
                    this.transform.position = Vector3.Lerp(start, dest, 1f - newPercent);
                }
            }, duration, EZTween.Curve.CubicEaseInOut);
            OnDidMove();
        }

        /// <summary>
        /// Normalizes rotation direction based on the current angle.
        /// </summary>
        /// <returns></returns>
        protected Vector2 NormalizedRotation(Vector2 dir) {
            dir.x *= -1;
            Vector2 rotated = Quaternion.AngleAxis(-_angle, Vector3.forward) * dir;

            Vector2 normalized = new Vector2(0, 0);
            if (rotated.x > 0.1) normalized.x = 1;
            else if (rotated.x < -0.1) normalized.x = -1;
            else if (rotated.y > 0.1) normalized.y = 1;
            else if (rotated.y < -0.1) normalized.y = -1;
            return normalized;
        }

        protected IEnumerator DoRotate(float newAngle, float duration) {
            Quaternion start = Quaternion.Euler(Vector3.up * _angle);
            Quaternion end = Quaternion.Euler(Vector3.up * newAngle);

            OnWillRotate(duration);
            yield return EZTween.DoPercentAction((float newPercent) => {
                this.transform.localRotation = Quaternion.Slerp(start, end, newPercent);
            }, duration, EZTween.Curve.CubicEaseInOut);
            OnDidRotate();

            _angle = newAngle;
        }

        protected virtual void OnWillMove(Vector3 destination, float duration) {

        }

        protected virtual void OnDidMove() {

        }

        protected virtual void OnWillRotate(float duration) {

        }

        protected virtual void OnDidRotate() {

        }

        protected virtual void OnWallBump() { }

        public void ResetEntity() {
            OnReset?.Invoke();
        }
    }
}