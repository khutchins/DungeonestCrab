namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Pathfinding/Costs/Standard")]
    public class CostStandardNode : TileCostNode {
        public override ITileCostProvider GetCostProvider() {
            return new StandardCostProvider();
        }

        public class StandardCostProvider : ITileCostProvider {
            public StandardCostProvider() { }
            public float GetCost(TheDungeon d, int x, int y) {
                var tileSpec = d.GetTileSpec(x, y);
                return tileSpec.TileCarvingCost;
            }
        }
    }
}