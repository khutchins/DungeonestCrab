using NUnit.Framework;
using UnityEngine;
using DungeonestCrab.Dungeon;
using System.Linq;
using System.Collections.Generic;

namespace DungeonestCrab.Tests {
    public class DungeonPatherTests {
        private TheDungeon _dungeon;

        [SetUp]
        public void Setup() {
            // 5x5 dungeon, boundaries are walls, interior is floor if we set it
            _dungeon = new TheDungeon(5, 5);
            for (int y = 1; y < 4; y++) {
                for (int x = 1; x < 4; x++) {
                    _dungeon.GetTileSpec(new Vector2Int(x, y)).Tile = Tile.Floor;
                }
            }
        }

        [Test]
        public void FindPath_FindsDirectPath_OnEmptyGrid() {
            var pather = new DungeonPather(_dungeon, DungeonPather.UniformCostWalkablePather());

            Vector2Int start = new Vector2Int(1, 1);
            Vector2Int end = new Vector2Int(3, 1);

            List<Vector2Int> path = pather.FindPath(start, end).ToList();

            Assert.IsNotNull(path);
            Assert.AreEqual(2, path.Count, "Path from (1,1) to (3,1) should have 2 steps excluding the start node");
            Assert.AreEqual(new Vector2Int(2, 1), path[0]);
            Assert.AreEqual(end, path[1]);
        }

        [Test]
        public void FindPath_RoutesAroundObstacles() {
            // Place a wall at (2,1)
            _dungeon.GetTileSpec(new Vector2Int(2, 1)).Tile = Tile.Wall;

            var pather = new DungeonPather(_dungeon, DungeonPather.UniformCostWalkablePather());

            Vector2Int start = new Vector2Int(1, 1);
            Vector2Int end = new Vector2Int(3, 1);

            List<Vector2Int> path = pather.FindPath(start, end).ToList();

            Assert.IsNotNull(path, "Path should be found by routing around the obstacle");
            Assert.IsTrue(path.Count > 3, "Path should be longer than the direct route due to obstacle");
            Assert.IsFalse(path.Contains(new Vector2Int(2, 1)), "Path should not go through the wall");
        }
    }
}
