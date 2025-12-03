using DungeonestCrab.Dungeon;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon {
    public class EntityTileModifierMixin : EntityMixin {
        [Title("Tile Modifications")]
        public bool ReplacesCeiling;
        public bool ReplacesFloor;
        public bool RemovesFloorCollider;
    }
}