namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Sources/Maze (Haphazard)")]
    public class SourceMazeHaphazardNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        public int MaxMoveDist = 3;
        public float InsertionAttempts = 2f;
        public int MoveMultplier = 1;

        public override ISource GetSource() => new SourceMazeHaphazard(TileToSet, MaxMoveDist, InsertionAttempts, MoveMultplier);
    }
}