using XNode;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    public abstract class ActionProviderNode : Node {
        [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)] public ActionConnection Output;
        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return GetAction();
            return null;
        }
        public abstract ITileAction GetAction();
    }
}
