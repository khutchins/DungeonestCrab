using DungeonestCrab.Dungeon;
using Pomerandomian;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [InlineEditor]
    public abstract class IFlatDrawer : ScriptableObject {
        public struct FlatInfo {
            public Transform parent;
            public IRandom random;
            public TileSpec tileSpec;
            public Vector3 tileSize;
        }

        public abstract GameObject DrawFlat(FlatInfo info);
    }
}