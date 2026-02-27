using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Definitions/Matchers/Logic/And")]
    public class AndMatcherNode : MatcherProviderNode {
        [Input(typeConstraint = TypeConstraint.Strict)] public MatcherConnection Matcher1;
        [Input(typeConstraint = TypeConstraint.Strict)] public MatcherConnection Matcher2;

        public override IMatcher GetMatcher() {
            var m1 = GetInputValues<IMatcher>("Matcher1", null);
            var m2 = GetInputValues<IMatcher>("Matcher2", null);

            List<IMatcher> matchers = new List<IMatcher>();
            foreach (var m in m1.Concat(m2)) {
                if (m != null) matchers.Add(m);
            }

            if (matchers.Count == 0) return TileMatcher.MatchingAll();
            if (matchers.Count == 1) return matchers[0];
            return new AndMatcher(matchers.ToArray());
        }
    }
}
