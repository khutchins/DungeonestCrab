using System.Collections;
using System.Collections.Generic;
using KH.Actions;
using Ratferences;
using UnityEngine;
using UnityEngine.Events;

namespace DungeonestCrab.Dungeon.Crawl {
    [DefaultExecutionOrder(-98)]
    public class DungeonInteractable : DungeonEntity, DungeonGrid.GridObject {
        [SerializeField] Action ActionOnTouch;

        public static readonly float KILL_OFFSET = 1000;

        public UnityEvent OnBeforeInteract;
        public UnityEvent OnAfterInteract;
        [SerializeField] bool _blocksMovement = true;
        [SerializeField] bool _faceOnInteract = true;
        [SerializeField] bool _isRegistered = true;
        private bool _awaitingAction;

        void Start() {
            UpdateRegistration();
            if (ActionOnTouch != null) {
                ActionOnTouch.FinishedAction += ActionFinished;
            }
            OnStart();
        }

        protected virtual void OnStart() {

        }

        public bool ShowOnMap {
            get => _blocksMovement;
        }

        public void UpdateRegistration() {
            if (!_isRegistered) {
                Vector3 transform = this.transform.position;
                transform.y += KILL_OFFSET;
                this.transform.position = transform;
            }
            DungeonGrid.GridItemInfo info = GridItemInfo();
            DungeonGrid.INSTANCE.RegisterEntity(this, info);
            _isRegistered = true;
        }

        public void UnKill() {
            UpdateRegistration();
        }

        public void Kill() {
            if (_isRegistered) {
                Vector3 transform = this.transform.position;
                transform.y -= KILL_OFFSET;
                this.transform.position = transform;
            }
            DungeonGrid.INSTANCE.UnregisterEntity(this);
            _isRegistered = false;
            //this.gameObject.SetActive(false);
        }

        protected virtual void DoAction() {
            if (ActionOnTouch != null) {
                ActionOnTouch.Begin();
            } else {
                ActionFinished(null);
            }
        }

        public IEnumerator HandleInteraction() {
            OnBeforeInteract?.Invoke();

            _awaitingAction = true;
            DoAction();

            while (_awaitingAction) yield return null;

            OnAfterInteract?.Invoke();
        }

        protected void ActionFinished(Action action) {
            _awaitingAction = false;
        }

        public bool BlocksMovement {
            get => _blocksMovement;
            set {
                _blocksMovement = value;
                DungeonGrid.INSTANCE.UpdateEntity(this);
            }
        }

        public bool TriggerOnMoveOnTile { get; set; }

        public bool IgnoreInteractButton { get; set; }

        public bool ForceTriggerOnBump { get; set; }

        public bool GetsMovementUpdates { get => false; }

        public bool FaceOnInteract {
            get => _faceOnInteract;
        }

        private void OnDrawGizmos() {
            Gizmos.color = BlocksMovement ? new Color(1, 0, 0, 0.4f) : new Color(0, 0, 1, 0.2f);
            float height = 0.5f;
            DungeonGrid.GridItemInfo info = GridItemInfo();
            if (info.OnEdge) {
                height *= 2;
                Gizmos.DrawCube(info.WorldPosition + Vector3.up * height / 2f, new Vector3(0.5f, height, 0.25f));
            } else {
                Gizmos.DrawCube(info.WorldPosition + Vector3.up * height / 2f, new Vector3(0.5f, height, 0.5f));
            }
        }
    }
}