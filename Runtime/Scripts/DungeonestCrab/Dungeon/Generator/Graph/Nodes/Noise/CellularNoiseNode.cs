using KH.Noise;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Noise/Cellular")]
    public class CellularNoiseNode : NoiseProviderNode {
        public float Frequency = 0.1f;
        public float Jitter = 1.0f;
        public FastNoiseLite.CellularDistanceFunction DistanceFunc = FastNoiseLite.CellularDistanceFunction.EuclideanSq;
        public FastNoiseLite.CellularReturnType ReturnType = FastNoiseLite.CellularReturnType.Distance;

        public override INoiseSource CreateNoiseSource() {
            return new NoiseSourceCellular(Frequency, Jitter, DistanceFunc, ReturnType);
        }
    }
}