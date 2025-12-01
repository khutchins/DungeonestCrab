namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Sources/Voronoi")]
    public class SourceVoronoiNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        public int Cells = 5;
        public int RegionSize = 4;
        public int Iterations = 3;

        public override ISource GetSource() => new SourceVoronoi(TileToSet, Cells, RegionSize, Iterations);
    }
}