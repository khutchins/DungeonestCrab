namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Actions/Stamp (With Terrain)")]
    public class StampTerrainNode : BaseStamperNode {
        public TerrainSO Terrain;

        protected override IAlterer CreateStamper(ISource source, Bounds bounds) {
            return new Stamper(source, Terrain, ProtectExistingTerrain, bounds, PreserveFloors, PreserveWalls);
        }
    }
}