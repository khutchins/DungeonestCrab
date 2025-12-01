using UnityEngine;
using KH.Noise;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Noise/Math/Add")]
    public class NoiseAddNode : NoiseProviderNode {
        [Input(ShowBackingValue.Never, ConnectionType.Multiple)] public NoiseConnection Sources;

        public override INoiseSource CreateNoiseSource() {
            INoiseSource[] inputs = GetInputValues<INoiseSource>("Sources", null);

            if (inputs == null || inputs.Length == 0) return new NoiseSourcePerlin(0.1f);

            return new DNoiseSourceAdd(inputs);
        }
    }
}