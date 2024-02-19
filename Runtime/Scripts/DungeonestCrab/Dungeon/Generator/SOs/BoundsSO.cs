using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [InlineEditor]
    public abstract class BoundsSO : ScriptableObject {
        public abstract Bounds ToBounds();
    }
}