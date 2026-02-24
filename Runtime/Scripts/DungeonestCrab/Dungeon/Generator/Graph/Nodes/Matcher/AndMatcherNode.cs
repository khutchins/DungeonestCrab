using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Matchers/And Matcher")]
    public class AndMatcherNode : MatcherProviderNode {
        [Input] public MatcherProviderNode Matcher1;
        [Input] public MatcherProviderNode Matcher2;

        public override IMatcher GetMatcher() {
            var m1 = GetInputValues<MatcherProviderNode>("Matcher1", Matcher1);
            var m2 = GetInputValues<MatcherProviderNode>("Matcher2", Matcher2);

            List<IMatcher> matchers = new List<IMatcher>();
            foreach (var m in m1.Concat(m2)) {
                if (m != null) matchers.Add(m.GetMatcher());
            }

            if (matchers.Count == 0) return TileMatcher.MatchingAll();
            if (matchers.Count == 1) return matchers[0];
            return new AndMatcher(matchers.ToArray());
        }
    }
}
