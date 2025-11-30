using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using KH.Noise;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Actions/Connect Regions (A*)")]
    public class AStarCombinerNode : DungeonPassNode {

        [Header("Inputs")]
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public NoiseConnection CarveNoise;

        [Header("Settings")]
        public TerrainSO FloorTerrain;
        [Range(0f, 1f)] public float ChanceOfExtraPath = 0f;

        [Header("Costs")]
        public float FloorCost = 1f;
        public float AdditionalCarveCost = 0f;
        public float NoiseMultiplier = 1f;

        protected override void ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            INoiseSource noise = GetInputValue<INoiseSource>("CarveNoise", null);

            IAlterer combiner = new AStarCombiner(
                FloorTerrain,
                ChanceOfExtraPath,
                FloorCost,
                AdditionalCarveCost,
                noise,
                NoiseMultiplier
            );

            combiner.Modify(dungeon, random);
        }
    }
}