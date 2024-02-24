using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Crawl {
    public class DungeonEntityMoverPath : DungeonEntityMover {
        private Vector2Int[] _path;
        private int _offset;

        public void SetPathAndOffset(Vector2Int[] path, int offset) {
            _path = path;
            _offset = offset;
            this.transform.position = DungeonGrid.INSTANCE.InfoForGridPosition(path[_offset]).WorldPosition;
            Rehome();
        }

        public override DungeonEntity.TurnAction GetTurnAction() {

            Vector2Int myPos = Vector2Int.RoundToInt(Entity.GridItemInfo().GridPosition);
            int idx = Array.IndexOf(_path, myPos);
            if (idx < 0) {
                Debug.LogWarning("It's off the grid!");
                return DungeonEntity.TurnAction.DoNothing;
            } else {
                Vector2Int from = At(idx);
                Vector2Int to = At(idx + 1);
                DungeonEntity.TurnAction action = DungeonGrid.INSTANCE.ActionForOffset(to - from);
                return action;
            }
        }

        private Vector2Int At(int idx) {
            return _path[(idx + _path.Length) % _path.Length];
        }
    }
}