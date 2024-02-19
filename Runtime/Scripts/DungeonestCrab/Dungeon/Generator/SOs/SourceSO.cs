using DungeonestCrab.Dungeon;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [InlineEditor]
    public abstract class SourceSO : ScriptableObject {
        public Tile TileToSet;

        public abstract ISource ToSource();
    }
}