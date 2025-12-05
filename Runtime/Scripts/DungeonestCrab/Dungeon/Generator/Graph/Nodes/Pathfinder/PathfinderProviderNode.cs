using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon.Generator;
using System.Collections.Generic;
using Pomerandomian;
using System.Linq;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    public abstract class PathfinderProviderNode : Node {
        [Output(ShowBackingValue.Never, ConnectionType.Multiple)] public PathfinderConnection Output;
        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return GetPathFinder();
            return null;
        }
        public abstract IPathFinder GetPathFinder();
    }
}