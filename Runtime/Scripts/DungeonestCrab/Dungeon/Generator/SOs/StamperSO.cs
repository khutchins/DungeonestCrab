using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "Dungeon/Spec/Stamper")]
    [InlineEditor]
    public class StamperSO : AltererSO {
        public SourceSO Source;
        public TerrainSO Terrain;
        public bool PassTerrains;
        public BoundsSO Bounds;

        public override IAlterer ToAlterer() {
            return new Stamper(Source.ToSource(), Terrain, PassTerrains, Bounds.ToBounds());
        }
    }
}