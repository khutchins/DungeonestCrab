using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "Dungeon/Spec/Source - Voronoi")]
    [InlineEditor]
    public class SourceVoronoiSO : SourceSO {
        [Range(0, 64)]
        public int Cells = 5;
        [Range(1, 64)]
        public int RegionSize = 4;
        [Range(0, 10)]
        public int Iterations = 3;

        public override ISource ToSource() {
            return new SourceVoronoi(TileToSet, Cells, RegionSize, Iterations);
        }
    }
}