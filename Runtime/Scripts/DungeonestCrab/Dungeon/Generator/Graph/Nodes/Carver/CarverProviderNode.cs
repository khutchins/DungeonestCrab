using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon.Generator;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    public abstract class CarverProviderNode : Node {
        [Output(ShowBackingValue.Never, ConnectionType.Multiple)] public CarverConnection Output;
        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return GetCarver();
            return null;
        }
        public abstract ITileCarver GetCarver();
    }
}