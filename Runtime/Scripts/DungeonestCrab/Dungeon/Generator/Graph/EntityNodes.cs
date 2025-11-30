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

    [CreateNodeMenu("Dungeon/Entities/Single Entity")]
    public class EntitySingleNode : EntityProviderNode {
        public EntitySO Entity;

        public override EntitySource GetSource() {
            if (Entity == null) return null;
            return EntitySource.Single(Entity);
        }
    }

    [CreateNodeMenu("Dungeon/Entities/Multi Entity")]
    public class EntityMultiNode : EntityProviderNode {
        public EntitySO[] Entities;

        public override EntitySource GetSource() {
            if (Entities == null || Entities.Length == 0) return null;
            return EntitySource.Multiple(Entities);
        }
    }
}