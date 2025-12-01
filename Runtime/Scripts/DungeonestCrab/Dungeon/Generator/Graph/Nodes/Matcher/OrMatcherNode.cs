using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
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