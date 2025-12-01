using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Sources/Cave")]
    public class SourceCaveNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        [Range(0, 1)] public float InitialOdds = 0.5f;
        public int MinNeighbors = 3;
        public int MaxNeighbors = 8;
        public int Iterations = 3;
        public bool Invert = false;

        public override ISource GetSource() => new SourceCave(TileToSet, InitialOdds, MinNeighbors, MaxNeighbors, Invert, Iterations);
    }
}