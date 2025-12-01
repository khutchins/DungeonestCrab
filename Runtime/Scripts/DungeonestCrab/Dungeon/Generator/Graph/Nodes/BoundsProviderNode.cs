using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    public abstract class BoundsProviderNode : BasePreviewNode {
        [Output(ShowBackingValue.Never, ConnectionType.Multiple)] public BoundsConnection Output;

        public abstract Bounds GetBounds();

        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return GetBounds();
            return null;
        }

        public Texture2D GetPreviewTexture() => PreviewTexture;

        public override void UpdatePreview() {
            Vector2Int size = GetDimensions();
            ValidatePreviewTexture(size.x, size.y);

            AppliedBounds full = new AppliedBounds(0, 0, size.x, size.y);
            Bounds b = GetBounds();
            AppliedBounds result = b.Apply(full);

            Color[] cols = new Color[size.x * size.y];
            for (int y = 0; y < size.y; y++) {
                for (int x = 0; x < size.x; x++) {
                    bool inside = x >= result.x && x < result.x + result.w &&
                                  y >= result.y && y < result.y + result.h;

                    cols[y * size.x + x] = inside ? Color.white : Color.black;
                }
            }
            PreviewTexture.SetPixels(cols);
            PreviewTexture.Apply();
        }
    }
}