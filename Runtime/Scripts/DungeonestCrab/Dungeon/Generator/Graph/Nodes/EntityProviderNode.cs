using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    public abstract class EntityProviderNode : Node {
        [Output(ShowBackingValue.Never, ConnectionType.Multiple)] public EntitySourceConnection Output;

        public abstract EntitySource GetSource();

        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return GetSource();
            return null;
        }
    }
}