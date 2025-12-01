using Pomerandomian;
using UnityEngine;
using XNode;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    public abstract class SourceProviderNode : BasePreviewNode {
        [Output(ShowBackingValue.Never, ConnectionType.Multiple)] public SourceConnection Output;

        public abstract ISource GetSource();

        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return GetSource();
            return null;
        }

        public Texture2D GetPreviewTexture() => PreviewTexture;

        public override void UpdatePreview() {
            Vector2Int size = GetDimensions();
            ValidatePreviewTexture(size.x, size.y);

            Stamp stamp = new Stamp(size.x, size.y);
            ISource source = GetSource();
            if (source != null) source.Generate(stamp, new SystemRandom(12345));

            Color[] cols = new Color[size.x * size.y];
            for (int y = 0; y < size.y; y++) {
                for (int x = 0; x < size.x; x++) {
                    Tile t = stamp.At(x, y);
                    if (t == Tile.Wall) cols[y * size.x + x] = Color.black;
                    else if (t == Tile.Floor) cols[y * size.x + x] = Color.white;
                    else cols[y * size.x + x] = new Color(0.25f, 0.25f, 0.25f);
                }
            }
            PreviewTexture.SetPixels(cols);
            PreviewTexture.Apply();
        }
    }
}