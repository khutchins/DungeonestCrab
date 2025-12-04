using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Matchers/Tile Matcher")]
    public class TileMatcherNode : MatcherProviderNode {
        [Header("Tile")]
        public bool CheckTile = true;
        public Tile Tile = Tile.Floor;

        [Header("Others")]
        [Tooltip("Will check terrain if set.")]
        public TerrainSO Terrain;
        [Tooltip("Will check tag if set.")]
        public string Tag = null;

        public override IMatcher GetMatcher() {
            return new TileMatcher(
                Tile, !CheckTile,
                Terrain, Terrain == null,
                Tag, string.IsNullOrEmpty(Tag)
            );
        }
    }
}