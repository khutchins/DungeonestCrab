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
            int w = 64; int h = 64;
            ValidatePreviewTexture(w, h);

            AppliedBounds full = new AppliedBounds(0, 0, w, h);
            Bounds b = GetBounds();
            AppliedBounds result = b.Apply(full);

            Color[] cols = new Color[w * h];
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    bool inside = x >= result.x && x < result.x + result.w &&
                                  y >= result.y && y < result.y + result.h;

                    cols[y * w + x] = inside ? Color.white : Color.black;
                }
            }
            PreviewTexture.SetPixels(cols);
            PreviewTexture.Apply();
        }
    }

    [CreateNodeMenu("Dungeon/Bounds/Full")]
    public class BoundsFullNode : BoundsProviderNode {
        public override Bounds GetBounds() => new FullBounds();
    }

    [CreateNodeMenu("Dungeon/Bounds/Inset (Fixed)")]
    public class BoundsInsetNode : BoundsProviderNode {
        public int Left = 1;
        public int Right = 1;
        public int Top = 1;
        public int Bottom = 1;

        public override Bounds GetBounds() => new InsetBounds(Left, Right, Top, Bottom);
    }

    [CreateNodeMenu("Dungeon/Bounds/Inset (Percent)")]
    public class BoundsInsetPercentNode : BoundsProviderNode {
        [Range(0, 1)] public float Left = 0.1f;
        [Range(0, 1)] public float Right = 0.1f;
        [Range(0, 1)] public float Top = 0.1f;
        [Range(0, 1)] public float Bottom = 0.1f;

        public override Bounds GetBounds() => new InsetPercentBounds(Left, Right, Top, Bottom);
    }

    [CreateNodeMenu("Dungeon/Bounds/Centered (Percent)")]
    public class BoundsCenteredNode : BoundsProviderNode {
        [Range(0, 1)] public float CenterX = 0.5f;
        [Range(0, 1)] public float CenterY = 0.5f;
        [Range(0, 1)] public float Width = 0.5f;
        [Range(0, 1)] public float Height = 0.5f;

        public override Bounds GetBounds() => new CenteredBounds(CenterX, CenterY, Width, Height);
    }
}