using Pomerandomian;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Pathfinding/Costs/Configurable")]
    public class CostConfigurableNode : TileCostNode {
        [Tooltip("The cost of traversing a tile that is already walkable.")]
        public float FloorCost = 1.0f;
        [Tooltip("Additional cost added to the base carving cost of walls/unset tiles.")]
        public float AdditionalCarveCost = 0.0f;
        [Tooltip("Multiplier applied to the base carving cost of the tile.")]
        public float BaseCostMultiplier = 1.0f;

        public override ITileCostProvider GetCostProvider() {
            return new ConfigurableCostProvider(FloorCost, AdditionalCarveCost, BaseCostMultiplier);
        }

        private class ConfigurableCostProvider : ITileCostProvider {
            readonly float _floorCost;
            readonly float _additionalCarveCost;
            readonly float _baseCostMultiplier;

            public ConfigurableCostProvider(float floorCost, float additionalCarveCost, float baseCostMultiplier) {
                _floorCost = floorCost;
                _additionalCarveCost = additionalCarveCost;
                _baseCostMultiplier = baseCostMultiplier;
            }

            public float GetCost(TheDungeon d, int x, int y) {
                TileSpec spec = d.GetTileSpec(x, y);
                
                if (spec.Walkable) return _floorCost;
                if (spec.Immutable) return -1;

                float baseCost = d.TileCarvingCost(spec);
                if (baseCost < 0) return baseCost;

                float cost = (baseCost * _baseCostMultiplier);
                if (d.GetTile(x, y) != Tile.Floor) {
                    cost += _additionalCarveCost;
                }
                
                return cost;
            }

            public void Init(IRandom rand) { }
        }
    }
}
