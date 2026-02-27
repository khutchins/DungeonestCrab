namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Pathfinding/Carvers/Standard")]
    public class CarverStandardNode : CarverProviderNode {
        public Tile TileToSet = Tile.Floor;
        public TerrainSO Terrain;
        public bool PreserveExistingFloors = false;

        public override ITileCarver GetCarver() => new CarverStandard(TileToSet, Terrain, PreserveExistingFloors);
    }
}