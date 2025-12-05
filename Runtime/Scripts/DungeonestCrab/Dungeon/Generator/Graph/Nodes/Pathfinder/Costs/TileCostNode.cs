using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon.Generator;
using KH.Noise;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    public interface ITileCostProvider {
        /// <summary>
        /// Calculates the cost to traverse or carve a specific tile.
        /// </summary>
        /// <returns>
        /// A value >= 0 for the cost (lower is better/faster). 
        /// A value < 0 (e.g. -1) indicates the tile is impassable/uncarvable.
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