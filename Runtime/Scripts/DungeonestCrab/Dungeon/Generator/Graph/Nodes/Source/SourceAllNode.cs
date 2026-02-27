namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Definitions/Sources/Geometric/Full Fill")]
    public class SourceAllNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        public override ISource GetSource() => new SourceAll(TileToSet);
    }
}