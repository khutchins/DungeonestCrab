using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Matchers/Not Matcher")]
    public class NotMatcherNode : MatcherProviderNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public MatcherConnection Matcher;

        public override IMatcher GetMatcher() {
            var input = GetInputValue<IMatcher>("Matcher", null);
            if (input == null) return null;
            return new NotMatcher(input);
        }
    }

    public class NotMatcher : IMatcher {
        private readonly IMatcher _matcher;
        public NotMatcher(IMatcher matcher) { _matcher = matcher; }
        public bool Matches(TileSpec tile) => !_matcher.Matches(tile);
    }
}
