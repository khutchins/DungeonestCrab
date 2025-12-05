using UnityEngine;
using KH.Noise;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Pathfinding/Costs/Add Noise")]
    public class CostAddNoiseNode : TileCostNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public CostProviderConnection Input;
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public NoiseConnection Noise;

        [Tooltip("How much influence the noise has. Higher values = more winding paths.")]
        public float Multiplier = 2.0f;

        public override ITileCostProvider GetCostProvider() {
            var upstream = GetInputValue<ITileCostProvider>("Input", null);
            var noise = GetInputValue<INoiseSource>("Noise", null);

            if (upstream == null) upstream = new CostStandardNode.StandardCostProvider();
            if (noise == null) return upstream;

            return new NoiseCostProvider(upstream, noise, Multiplier);
        }

        private class NoiseCostProvider : ITileCostProvider {
            readonly ITileCostProvider _baseProvider;
            readonly INoiseSource _noise;
            readonly float _multiplier;
            public NoiseCostProvider(ITileCostProvider baseProvider, INoiseSource noise, float multiplier) {
                _baseProvider = baseProvider;
                _noise = noise;
                _multiplier = multiplier;
            }

            public float GetCost(TheDungeon d, int x, int y) {
                float c = _baseProvider.GetCost(d, x, y);
                // Don't accidentally make impassable tiles passable.
                if (c < 0) return c;
                return c + (_noise.At(x, y) * _multiplier);
            }
        }
    }
}