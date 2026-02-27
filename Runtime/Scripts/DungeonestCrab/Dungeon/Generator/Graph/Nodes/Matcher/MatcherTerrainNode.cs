namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Definitions/Matchers/Match Terrain")]
    public class MatcherTerrainNode : MatcherProviderNode {
        public TerrainSO Terrain;

        public override IMatcher GetMatcher() {
            return new TileMatcher(Tile.Unset, true, Terrain, false, null, true);
        }
    }
}