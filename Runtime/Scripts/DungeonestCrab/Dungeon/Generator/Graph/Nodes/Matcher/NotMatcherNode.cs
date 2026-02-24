using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Matchers/Not Matcher")]
    public class NotMatcherNode : MatcherProviderNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public MatcherProviderNode Matcher;

        public override IMatcher GetMatcher() {
            var input = GetInputValue<MatcherProviderNode>("Matcher", Matcher);
            if (input == null) return null;
            return new NotMatcher(input.GetMatcher());
        }
    }

    public class NotMatcher : IMatcher {
        private readonly IMatcher _matcher;
        public NotMatcher(IMatcher matcher) { _matcher = matcher; }
        public bool Matches(TileSpec tile) => !_matcher.Matches(tile);
    }
}
