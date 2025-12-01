using UnityEngine;
using KH.Noise;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Noise/Math/Invert")]
    public class NoiseInvertNode : NoiseProviderNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public NoiseConnection Input;

        public override INoiseSource CreateNoiseSource() {
            INoiseSource src = GetInputValue<INoiseSource>("Input", null);
            if (src == null) return new NoiseSourcePerlin(0.1f); // Fallback

            return new DNoiseSourceInvert(src);
        }
    }
}