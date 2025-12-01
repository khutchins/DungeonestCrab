namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Sources/All")]
    public class SourceAllNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        public override ISource GetSource() => new SourceAll(TileToSet);
    }
}