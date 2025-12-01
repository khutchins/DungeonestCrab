using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
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
        public string Style = null;

        public override IMatcher GetMatcher() {
            return new TileMatcher(
                Tile, !CheckTile,
                Terrain, !CheckTerrain,
                Style, !CheckStyle
            );
        }
    }
}