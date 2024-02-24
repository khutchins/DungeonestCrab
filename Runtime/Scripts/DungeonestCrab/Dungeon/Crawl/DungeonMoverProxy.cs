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
        private DungeonMoverReference DungeonMover;
        public void TryMoveLeft() {
            DungeonMover.Value.TryMoveLeft();
        }

        public void TryMoveRight() {
            DungeonMover.Value.TryMoveRight();
        }

        public void TryMoveForward() {
            DungeonMover.Value.TryMoveForward();
        }

        public void TryMoveBack() {
            DungeonMover.Value.TryMoveBack();
        }

        public void TryTurnLeft() {
            DungeonMover.Value.TryTurnLeft();
        }

        public void TryTurnRight() {
            DungeonMover.Value.TryTurnRight();
        }

        public void TryWait() {
            DungeonMover.Value.TryWait();
        }
    }
}