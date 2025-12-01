using KH.Noise;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Noise/Perlin")]
    public class PerlinNoiseNode : NoiseProviderNode {
        public float Frequency = 0.1f;

        public override INoiseSource CreateNoiseSource() {
            return new NoiseSourcePerlin(Frequency);
        }
    }
}