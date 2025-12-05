using DungeonestCrab.Dungeon.Generator;
using KH.Noise;
using Pomerandomian;
using UnityEngine;
using XNode;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    public interface ITileCostProvider {
        /// <summary>
        /// Called at the start of pathfinding. Use this to set seeds on noise functions.
        /// </summary>
        void Init(IRandom rand);

        /// <summary>
        /// Calculates the cost to traverse or carve a specific tile. Negative is impassable.
        /// </returns>
        float GetCost(TheDungeon d, int x, int y);
    }


    public abstract class TileCostNode : Node {
        [Output(ShowBackingValue.Never, ConnectionType.Multiple)] public CostProviderConnection Output;
        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return GetCostProvider();
            return null;
        }
        public abstract ITileCostProvider GetCostProvider();
    }
}