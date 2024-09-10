using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "DungeonestCrab/Spec/TerrainSetter")]
    [InlineEditor]
    public class TerrainSetterSO : AltererSO {
        [SerializeField] MatcherSO Matcher;
        [SerializeField] int Range = 1;
        [SerializeField] TerrainSO Terrain;

        public override IAlterer ToAlterer() {
            return new TerrainSetter(Matcher.ToMatcher(), Range, Terrain);
        }
    }
}