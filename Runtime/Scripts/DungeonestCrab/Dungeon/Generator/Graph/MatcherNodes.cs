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
            int w = 64; int h = 64;
            ValidatePreviewTexture(w, h);

            IMatcher matcher = GetMatcher();

            // Create dummy tile for testing
            TileSpec floorSpec = new TileSpec(Vector2Int.zero, Tile.Floor, null, 0, false);
            TileSpec wallSpec = new TileSpec(Vector2Int.zero, Tile.Wall, null, 0, false);

            Color cMatch = Color.white;
            Color cNoMatch = Color.black;

            Color[] cols = new Color[w * h];
            for (int i = 0; i < cols.Length; i++) {
                bool isWall = (i % 2 == 0);
                cols[i] = matcher.Matches(isWall ? wallSpec : floorSpec) ? cMatch : cNoMatch;
            }

            PreviewTexture.SetPixels(cols);
            PreviewTexture.Apply();
        }
    }

    [CreateNodeMenu("Dungeon/Matchers/Tile Matcher")]
    public class TileMatcherNode : MatcherProviderNode {
        [Header("Tile")]
        public bool CheckTile = true;
        public Tile Tile = Tile.Floor;

        [Header("Terrain")]
        public bool CheckTerrain = false;
        public TerrainSO Terrain;

        [Header("Style")]
        public bool CheckStyle = false;
        public int Style = 0;

        public override IMatcher GetMatcher() {
            return new TileMatcher(
                Tile, !CheckTile,
                Terrain, !CheckTerrain,
                Style, !CheckStyle
            );
        }
    }

    [CreateNodeMenu("Dungeon/Matchers/Tile")]
    public class MatcherTileNode : MatcherProviderNode {
        public Tile Tile = Tile.Floor;

        public override IMatcher GetMatcher() {
            return new TileMatcher(Tile, false, null, true, 0, true);
        }
    }

    [CreateNodeMenu("Dungeon/Matchers/Terrain")]
    public class MatcherTerrainNode : MatcherProviderNode {
        public TerrainSO Terrain;

        public override IMatcher GetMatcher() {
            return new TileMatcher(Tile.Unset, true, Terrain, false, 0, true);
        }
    }

    [CreateNodeMenu("Dungeon/Matchers/Style")]
    public class MatcherStyleNode : MatcherProviderNode {
        public int Style = 0;

        public override IMatcher GetMatcher() {
            return new TileMatcher(Tile.Unset, true, null, true, Style, false);
        }
    }

    [CreateNodeMenu("Dungeon/Matchers/Or Matcher")]
    public class OrMatcherNode : MatcherProviderNode {
        [Input(ShowBackingValue.Never, ConnectionType.Multiple)] public MatcherConnection Matchers;

        public override IMatcher GetMatcher() {
            var inputs = GetInputValues<IMatcher>("Matchers", null);
            if (inputs == null || inputs.Length == 0) return TileMatcher.MatchingAll();
            return new OrMatcher(inputs);
        }
    }
}