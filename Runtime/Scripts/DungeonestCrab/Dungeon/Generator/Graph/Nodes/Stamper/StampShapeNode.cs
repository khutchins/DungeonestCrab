namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Actions/Stamp (Shape Only)")]
    public class StampShapeNode : BaseStamperNode {
        protected override IAlterer CreateStamper(ISource source, Bounds bounds) {
            return new Stamper(source, null, ProtectExistingTerrain, bounds, PreserveFloors, PreserveWalls);
        }
    }
}