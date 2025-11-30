using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Sources/Maze")]
    public class MazeNode : DungeonPassNode {
        [Header("Maze Settings")]
        public Tile TileToSet = Tile.Floor;
        [Range(0f, 1f)] public float StraightBias = 0.5f;
        public bool Conservative = false;
        [Range(0f, 1f)] public float BraidPercent = 0f;

        protected override void ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            ISource source = new SourceMaze(TileToSet, StraightBias, Conservative, BraidPercent);
            Stamp stamp = new Stamp(dungeon, false, dungeon.Bounds);
            source.Generate(stamp, random);

            foreach (Vector2Int pt in stamp.All()) {
                Tile t = stamp.At(pt);
                if (t != Tile.Unset) {
                    TileSpec spec = dungeon.GetTileSpec(pt);
                    if (!spec.Immutable) {
                        spec.Tile = t;
                    }
                }
            }
        }
    }
}