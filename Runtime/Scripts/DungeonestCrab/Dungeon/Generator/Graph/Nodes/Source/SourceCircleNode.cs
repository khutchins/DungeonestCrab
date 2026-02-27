namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Definitions/Sources/Geometric/Circle")]
    public class SourceCircleNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        public override ISource GetSource() => new SourceCircle(TileToSet);
    }
}