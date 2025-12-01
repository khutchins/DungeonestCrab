namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Matchers/Tile")]
    public class MatcherTileNode : MatcherProviderNode {
        public Tile Tile = Tile.Floor;

        public override IMatcher GetMatcher() {
            return new TileMatcher(Tile, false, null, true, null, true);
        }
    }
}