namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Definitions/Matchers/Match Tag")]
    public class MatcherStyleNode : MatcherProviderNode {
        public string Style = null;

        public override IMatcher GetMatcher() {
            return new TileMatcher(Tile.Unset, true, null, true, Style, false);
        }
    }
}