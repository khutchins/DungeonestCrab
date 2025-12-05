namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Pathfinding/Pather: A*")]
    public class PathfinderAStarNode : PathfinderProviderNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public CostProviderConnection CostFunction;

        public override IPathFinder GetPathFinder() {
            var costProvider = GetInputValue<ITileCostProvider>("CostFunction", null);

            // Default to dungeon logic.
            costProvider ??= new DefaultDungeonCostProvider();

            return new PathfinderAStar((d, x, y) => costProvider.GetCost(d, x, y));
        }

        private class DefaultDungeonCostProvider : ITileCostProvider {
            public float GetCost(TheDungeon d, int x, int y) {
                TileSpec spec = d.GetTileSpec(x, y);
                return d.TileCarvingCost(spec);
            }
        }
    }
}