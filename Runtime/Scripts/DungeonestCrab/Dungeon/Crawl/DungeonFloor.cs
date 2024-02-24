using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Crawl {
    /// <summary>
    /// Registers the given node as a valid movement position.
    /// </summary>
    [DefaultExecutionOrder(-99)]
    public class DungeonFloor : MonoBehaviour, DungeonGrid.GridObject {
        private DungeonGrid.GridItemInfo _gridInfo;

        void Awake() {
            _gridInfo = DungeonGrid.INSTANCE.InfoForLocation(transform.position);
            DungeonGrid.INSTANCE.RegisterNode(this);
        }

        public DungeonGrid.GridItemInfo GridItemInfo() {
            return _gridInfo;
        }

        private void OnDrawGizmos() {
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            DungeonGrid instance = DungeonGrid.INSTANCE;
            Vector2 size = (instance != null ? instance.CellSize : Vector2.one) * 0.9f;
            Gizmos.DrawCube(_gridInfo.WorldPosition, new Vector3(size.x, 0.1f, size.y));
            if (Vector3.Distance(transform.position, _gridInfo.WorldPosition) > 0.5f) {
                Gizmos.color = new Color(1, 0, 0, 0.8f);
                Gizmos.DrawLine(transform.position, _gridInfo.WorldPosition);
            }
        }
    }
}