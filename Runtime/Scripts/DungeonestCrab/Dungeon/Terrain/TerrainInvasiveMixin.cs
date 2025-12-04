using DungeonestCrab.Dungeon;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace DungeonestCrab.Dungeon {

    [Serializable]
    public class TerrainInvasiveMixin : TerrainMixin {
        [Title("Invasive Settings")]
        [Tooltip("If true, this terrain will 'win' when seeing who will draw an upper wall.")]
        public bool HasPriorityUpperWall = true;
        [Tooltip("If true, this terrain will 'win' when seeing who will draw a standard wall.")]
        public bool HasPriorityWall = true;
        [Tooltip("If true, this terrain will 'win' when seeing who will draw a lower wall.")]
        public bool HasPriorityLowerWall = true;
    }
}