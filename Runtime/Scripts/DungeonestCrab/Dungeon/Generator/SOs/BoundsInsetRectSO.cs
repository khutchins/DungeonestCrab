using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "Dungeon/Spec/Bounds - Inset Rect")]
    public class BoundsInsetRectSO : BoundsSO {
        public Vector2Int Position = Vector2Int.zero;
        public Vector2Int Size = Vector2Int.one;

        public override Bounds ToBounds() {
            return new FixedBounds(Position.x, Position.y, Size.x, Size.y);
        }
    }
}