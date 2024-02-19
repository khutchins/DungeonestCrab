using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "Dungeon/Spec/Source - Stripes")]
    [InlineEditor]
    public class SourceStripesSO : SourceSO {
        [Range(0f, 1f)]
        public float DensityPerSide = 0.2f;
        [Range(0f, 1f)]
        public float CurveOdds = 0.1f;
        [Range(0f, 1f)]
        public float DeadEndOdds = 0.1f;

        public override ISource ToSource() {
            return new SourceStripe(TileToSet, DensityPerSide, CurveOdds, DeadEndOdds);
        }
    }
}