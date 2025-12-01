using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    public abstract class MatcherProviderNode : BasePreviewNode {
        [Output(ShowBackingValue.Never, ConnectionType.Multiple)] public MatcherConnection Output;

        public abstract IMatcher GetMatcher();

        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return GetMatcher();
            return null;
        }

        // Visualize what matches against a generic floor grid, which probably isn't very interesting.
        public Texture2D GetPreviewTexture() => PreviewTexture;
        public override void UpdatePreview() {
            Vector2Int size = GetDimensions();
            ValidatePreviewTexture(size.x, size.y);

            IMatcher matcher = GetMatcher();

            // Create dummy tile for testing
            TileSpec floorSpec = new TileSpec(Vector2Int.zero, Tile.Floor, null, false);
            TileSpec wallSpec = new TileSpec(Vector2Int.zero, Tile.Wall, null, false);

            Color cMatch = Color.white;
            Color cNoMatch = Color.black;

            Color[] cols = new Color[size.x * size.y];
            for (int i = 0; i < cols.Length; i++) {
                bool isWall = (i % 2 == 0);
                cols[i] = matcher.Matches(isWall ? wallSpec : floorSpec) ? cMatch : cNoMatch;
            }

            PreviewTexture.SetPixels(cols);
            PreviewTexture.Apply();
        }
    }
}