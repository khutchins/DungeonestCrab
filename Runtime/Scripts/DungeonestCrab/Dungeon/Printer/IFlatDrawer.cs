using DungeonestCrab.Dungeon;
using Pomerandomian;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [InlineEditor]
    public abstract class IFlatDrawer : ScriptableObject {
        public abstract GameObject DrawFlat(Transform parent, IRandom random, TileSpec tileSpec);
    }
}