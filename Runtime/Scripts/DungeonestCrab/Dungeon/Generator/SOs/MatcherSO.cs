using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [InlineEditor]
    public abstract class MatcherSO : ScriptableObject {

        public abstract IMatcher ToMatcher();


    }
}