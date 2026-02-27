using Pomerandomian;
using System.Collections.Generic;
using XNode;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Actions/Multi-Action")]
    public class ActionMultiNode : ActionProviderNode {
        [Input(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)] public ActionConnection SubActions;
        public override ITileAction GetAction() {
            var inputs = GetInputValues<ITileAction>("SubActions", null);
            return new MultiAction(inputs != null ? new List<ITileAction>(inputs) : new List<ITileAction>());
        }

        private class MultiAction : ITileAction {
            readonly List<ITileAction> _actions;
            public MultiAction(List<ITileAction> actions) => _actions = actions;
            public void Apply(TileSpec spec, IRandom rand) {
                foreach (var a in _actions) a?.Apply(spec, rand);
            }
        }
    }
}
