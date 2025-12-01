using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Sources/Maze")]
    public class SourceMazeNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        [Range(0, 1)] public float StraightBias = 0.5f;
        [Range(0, 1)] public float BraidPercent = 0f;
        public bool Conservative = false;

        public override ISource GetSource() => new SourceMaze(TileToSet, StraightBias, Conservative, BraidPercent);
    }
}