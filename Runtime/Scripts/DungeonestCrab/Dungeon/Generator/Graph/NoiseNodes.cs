using UnityEngine;
using XNode;
using KH.Noise;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    public abstract class NoiseProviderNode : BasePreviewNode {
        [Output(ShowBackingValue.Never, ConnectionType.Multiple)] public NoiseConnection Output;

        public abstract INoiseSource CreateNoiseSource();

        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return CreateNoiseSource();
            return null;
        }
        public Texture2D GetPreviewTexture() => PreviewTexture;

        public override void UpdatePreview() {
            Vector2Int size = GetDimensions();
            ValidatePreviewTexture(size.x, size.y);

            INoiseSource noise = CreateNoiseSource();

            noise.SetSeed(12345);

            Color[] cols = new Color[size.x * size.y];
            for (int y = 0; y < size.y; y++) {
                for (int x = 0; x < size.x; x++) {
                    float val = noise.At(x, y);
                    cols[y * size.x + x] = new Color(val, val, val);
                }
            }
            PreviewTexture.SetPixels(cols);
            PreviewTexture.Apply();
        }
    }

    [CreateNodeMenu("Dungeon/Noise/Perlin")]
    public class PerlinNoiseNode : NoiseProviderNode {
        public float Frequency = 0.1f;

        public override INoiseSource CreateNoiseSource() {
            return new NoiseSourcePerlin(Frequency);
        }
    }

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