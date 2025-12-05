namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Pathfinding/Carver: Standard")]
    public class CarverStandardNode : CarverProviderNode {
        public Tile TileToSet = Tile.Floor;
        public TerrainSO Terrain;

        public override ITileCarver GetCarver() => new CarverStandard(TileToSet, Terrain);
    }
}