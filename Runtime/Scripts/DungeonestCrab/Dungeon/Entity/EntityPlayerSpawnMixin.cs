using DungeonestCrab.Dungeon;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon {
    public class EntityPlayerSpawnMixin : EntityMixin {
        [Title("Spawner Adjustments")]
        public float Offset = 0f;
    }
}