using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "DungeonestCrab/Spec/Matcher - Tile")]
    public class TileMatcherSO : MatcherSO {
        [SerializeField] bool HasTileCriteria;
        [ShowIf("HasTileCriteria")]
        [SerializeField] Tile _tile;
        [SerializeField] bool HasTerrainCriteria;
        [ShowIf("HasTerrainCriteria")]
        [SerializeField] TerrainSO _terrain;
        [SerializeField] bool HasStyleCriteria;
        [ShowIf("HasStyleCriteria")]
        [SerializeField] int _style;

        public override IMatcher ToMatcher() {
            return new TileMatcher(_tile, !HasTileCriteria, _terrain, !HasTerrainCriteria, _style, !HasStyleCriteria);
        }
    }
}
