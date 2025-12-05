using Pomerandomian;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Pathfinding/Costs/Restrict to Terrain")]
    public class CostRestrictTerrainNode : TileCostNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public CostProviderConnection Input;

        [Tooltip("If set, pathfinding is only allowed on this terrain.")]
        public TerrainSO RequiredTerrain;
        [Tooltip("If set, tiles that are walkable may also be traversed.")]
        public bool AllowExistingWalkable = true;

        public override ITileCostProvider GetCostProvider() {
            var upstream = GetInputValue<ITileCostProvider>("Input", null);
            // Default to standard costs if no input provided
            if (upstream == null) upstream = new CostStandardNode.StandardCostProvider();

            return new RestrictTerrainProvider(upstream, RequiredTerrain, AllowExistingWalkable);
        }

        private class RestrictTerrainProvider : ITileCostProvider {
            ITileCostProvider _baseProvider;
            TerrainSO _terrain;
            bool _allowExistingWalkable;

            public RestrictTerrainProvider(ITileCostProvider @base, TerrainSO terrain, bool allowExistingWalkable) {
                _baseProvider = @base;
                _terrain = terrain;
                _allowExistingWalkable = allowExistingWalkable;
            }

            public float GetCost(TheDungeon dungeon, int x, int y) {
                var tileSpec = dungeon.GetTileSpec(x, y);
                // If it's already floor and we allow walking on existing floors
                if (_allowExistingWalkable && tileSpec.Walkable) return _baseProvider.GetCost(dungeon, x, y);

                // If it's a wall (or unset), ensure the terrain matches.
                if (tileSpec.Terrain != _terrain) return -1;

                return _baseProvider.GetCost(dungeon, x, y);
            }

            public void Init(IRandom rand) { }
        }
    }
}