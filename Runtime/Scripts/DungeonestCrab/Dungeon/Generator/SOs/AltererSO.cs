using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    public abstract class AltererSO : ScriptableObject {
        public abstract IAlterer ToAlterer();
    }
}