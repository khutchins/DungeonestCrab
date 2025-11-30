using UnityEngine;
using XNode;
using KH.Noise;
using Pomerandomian;

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

    [CreateNodeMenu("Dungeon/Noise/Math/Add")]
    public class NoiseAddNode : NoiseProviderNode {
        [Input(ShowBackingValue.Never, ConnectionType.Multiple)] public NoiseConnection Sources;

        public override INoiseSource CreateNoiseSource() {
            INoiseSource[] inputs = GetInputValues<INoiseSource>("Sources", null);

            if (inputs == null || inputs.Length == 0) return new NoiseSourcePerlin(0.1f);

            return new DNoiseSourceAdd(inputs);
        }
    }

    [CreateNodeMenu("Dungeon/Noise/Math/Mix (Average)")]
    public class NoiseMixNode : NoiseProviderNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public NoiseConnection A;
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public NoiseConnection B;

        [Tooltip("0 = All B, 1 = All A")]
        [Range(0, 1)] public float WeightA = 0.5f;

        public override INoiseSource CreateNoiseSource() {
            INoiseSource srcA = GetInputValue<INoiseSource>("A", null);
            INoiseSource srcB = GetInputValue<INoiseSource>("B", null);

            if (srcA == null && srcB == null) return new NoiseSourcePerlin(0.1f);
            if (srcA == null) return srcB;
            if (srcB == null) return srcA;

            return new DNoiseSourceAvg(srcA, srcB, WeightA);
        }
    }
}