namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Definitions/Matchers/Match Tile Type")]
    public class MatcherTileNode : MatcherProviderNode {
        public Tile Tile = Tile.Floor;

        public override IMatcher GetMatcher() {
            return new TileMatcher(Tile, false, null, true, null, true);
        }
    }
}