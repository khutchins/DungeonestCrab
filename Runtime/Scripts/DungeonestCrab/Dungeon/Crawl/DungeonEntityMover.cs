using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Crawl {
    [RequireComponent(typeof(DungeonEntity))]
    public abstract class DungeonEntityMover : MonoBehaviour {

        protected DungeonEntity _dungeonEntity;

        protected DungeonEntity Entity {
            get {
                if (_dungeonEntity == null) {
                    _dungeonEntity = GetComponent<DungeonEntity>();
                    _dungeonEntity.OnReset.AddListener(OnReset);
                }
                return _dungeonEntity;
            }
        }

        protected Vector2 _home;

        private void Awake() {
            _home = Entity.GridItemInfo().GridPosition;
            ExtraAwake();
        }

        protected void Rehome() {
            _home = Entity.GridItemInfo().GridPosition;
        }

        protected virtual void ExtraAwake() {

        }

        public abstract DungeonEntity.TurnAction GetTurnAction();

        public void OnReset() {
            DungeonGrid.GridItemInfo info = DungeonGrid.INSTANCE.InfoForGridPosition(_home);
            this.transform.position = info.WorldPosition;
            ExtraReset();
            if (_dungeonEntity is DungeonInteractable interact) interact.UpdateRegistration();
        }

        protected virtual void ExtraReset() {

        }
    }
}