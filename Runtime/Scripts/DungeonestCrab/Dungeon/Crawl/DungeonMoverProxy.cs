using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Crawl {
    /// <summary>
    /// Proxy used for calling out to DungeonMover movement
    /// from the inspector, like for hooking up buttons.
    /// </summary>
    public class DungeonMoverProxy : MonoBehaviour {

        [SerializeField]
        private DungeonEntityReference DungeonMover;

        private DungeonMover Mover {
            get {
                DungeonEntity entity = DungeonMover.Value;
                if (entity == null) return null;
                if (!(entity is DungeonMover)) return null;
                return entity as DungeonMover;
            }
        }

        public void TryDoAction(DungeonMover.Action action) {
            var mover = Mover;
            if (mover == null) {
                Debug.LogWarning($"Trying to do action but dungeon mover proxy isn't attached!");
                return;
            }
            mover.TryDoAction(action);
        }

        public void TryMoveLeft() {
            TryDoAction(Crawl.DungeonMover.Action.MoveLeft);
        }

        public void TryMoveRight() {
            TryDoAction(Crawl.DungeonMover.Action.MoveRight);
        }

        public void TryMoveForward() {
            TryDoAction(Crawl.DungeonMover.Action.MoveForward);
        }

        public void TryMoveBack() {
            TryDoAction(Crawl.DungeonMover.Action.MoveBack);
        }

        public void TryTurnLeft() {
            TryDoAction(Crawl.DungeonMover.Action.TurnLeft);
        }

        public void TryTurnRight() {
            TryDoAction(Crawl.DungeonMover.Action.TurnRight);
        }

        public void TryWait() {
            TryDoAction(Crawl.DungeonMover.Action.Wait);
        }
    }
}