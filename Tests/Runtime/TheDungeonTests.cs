using NUnit.Framework;
using UnityEngine;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Tests {
    public class TheDungeonTests {
        private TheDungeon _dungeon;

        [SetUp]
        public void Setup() {
            _dungeon = new TheDungeon(5, 5);
        }

        [Test]
        public void GetTileSpec_ReturnsCorrectTile() {
            TileSpec spec = _dungeon.GetTileSpec(new Vector2Int(2, 2));
            Assert.IsNotNull(spec);
            Assert.AreEqual(new Vector2Int(2, 2), spec.Coords, "Coordinates should match the requested vector");
            Assert.AreEqual(Tile.Wall, spec.Tile, "Interior tiles should initialize as wall by default");
        }

        [Test]
        public void WalkableAt_RespectsBaseWalkability() {
            Vector2Int pos = new Vector2Int(2, 2);
            TileSpec spec = _dungeon.GetTileSpec(pos);

            spec.Tile = Tile.Floor;
            Assert.IsTrue(_dungeon.WalkableAt(pos), "Floor should be walkable");

            spec.Tile = Tile.Wall;
            Assert.IsFalse(_dungeon.WalkableAt(pos), "Wall should not be walkable");
        }

        [Test]
        public void FindWalkableDir_ReturnsCorrectDirection() {
            Vector2Int pos = new Vector2Int(2, 2);
            TileSpec spec = _dungeon.GetTileSpec(pos);

            // Set all adjacent to wall
            _dungeon.GetTileSpec(new Vector2Int(2, 3)).Tile = Tile.Wall; // N
            _dungeon.GetTileSpec(new Vector2Int(3, 2)).Tile = Tile.Floor; // E
            _dungeon.GetTileSpec(new Vector2Int(2, 1)).Tile = Tile.Wall; // S
            _dungeon.GetTileSpec(new Vector2Int(1, 2)).Tile = Tile.Wall; // W

            string dir = _dungeon.FindWalkableDir(spec);
            Assert.AreEqual("E", dir, "FindWalkableDir should return E when only East is walkable");
        }

        [Test]
        public void InvalidateCaches_ResetsFields() {
            // Make a tile walkable so there is exactly one region
            _dungeon.GetTileSpec(new Vector2Int(2, 2)).Tile = Tile.Floor;

            // Compute regions to populate cache.
            _dungeon.ComputeRegions(out _);
            Assert.AreEqual(1, _dungeon.ComputeRegionCount(), "Should have exactly 1 region");

            // Invalidate the cache and remove that floor
            _dungeon.InvalidateCaches();
            _dungeon.GetTileSpec(new Vector2Int(2, 2)).Tile = Tile.Wall;

            int newCount = _dungeon.ComputeRegionCount();
            Assert.AreEqual(0, _dungeon.ComputeRegionCount());
        }
    }
}
