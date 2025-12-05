using XNode;
using DungeonestCrab.Dungeon.Generator;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    public abstract class ConnectorProviderNode : Node {
        [Output(ShowBackingValue.Never, ConnectionType.Multiple)] public ConnectorConnection Output;
        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return GetConnector();
            return null;
        }
        public abstract IRegionConnector GetConnector();
    }
}