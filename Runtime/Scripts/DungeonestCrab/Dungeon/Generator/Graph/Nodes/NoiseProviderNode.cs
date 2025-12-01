using UnityEngine;
using XNode;
using KH.Noise;

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

            noise.SetSeed(GetPreviewSeed());

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
}