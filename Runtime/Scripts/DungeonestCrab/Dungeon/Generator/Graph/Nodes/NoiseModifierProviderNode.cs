using UnityEngine;
using XNode;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    public abstract class NoiseModifierProviderNode : BasePreviewNode {
        [Output(ShowBackingValue.Never, ConnectionType.Multiple)] public NoiseModifierConnection Output;

        public abstract INoiseModifier CreateModifier();

        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return CreateModifier();
            return null;
        }

        public Texture2D GetPreviewTexture() => PreviewTexture;

        public override void UpdatePreview() {
            Vector2Int size = GetDimensions();
            ValidatePreviewTexture(size.x, size.y);

            INoiseModifier modifier = CreateModifier();

            AppliedBounds bounds = new AppliedBounds(0, 0, size.x, size.y);

            Color[] cols = new Color[size.x * size.y];
            for (int y = 0; y < size.y; y++) {
                for (int x = 0; x < size.x; x++) {
                    float val = modifier.Modifier(bounds, x, y);
                    // We visualize subtracting the modifier, so 0 is white (no subtraction), 1 is black (full subtraction)
                    val = 1 - Mathf.Clamp01(val);
                    cols[y * size.x + x] = new Color(val, val, val);
                }
            }
            PreviewTexture.SetPixels(cols);
            PreviewTexture.Apply();
        }
    }
}
