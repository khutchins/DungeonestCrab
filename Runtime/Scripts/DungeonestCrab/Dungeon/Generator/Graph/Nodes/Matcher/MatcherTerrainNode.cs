namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Matchers/Terrain")]
    public class MatcherTerrainNode : MatcherProviderNode {
        public TerrainSO Terrain;

        public override IMatcher GetMatcher() {
            return new TileMatcher(Tile.Unset, true, Terrain, false, 0, true);
        }
    }
}