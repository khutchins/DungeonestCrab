namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Sources/River")]
    public class SourceRiverNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        public float MinWidth = 0.5f;
        public float MaxWidth = 1.5f;
        public SourceRiver.Sides StartSides = SourceRiver.Sides.Top;
        public SourceRiver.Sides EndSides = SourceRiver.Sides.Bottom;

        public override ISource GetSource() => new SourceRiver(TileToSet, MinWidth, MaxWidth, StartSides, EndSides);
    }
}