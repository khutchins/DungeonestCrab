using NUnit.Framework;
using UnityEngine;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Tests {
    public class TileSpecTests {
        private TileSpec _spec;

        [SetUp]
        public void Setup() {
            var terrain = ScriptableObject.CreateInstance<TerrainSO>();
            _spec = new TileSpec(Vector2Int.zero, Tile.Floor, terrain, false);
        }

        [Test]
        public void AddTag_And_HasTag_WorkCorrectly() {
            string testTag = "test:tag";
            Assert.IsFalse(_spec.HasTag(testTag), "Should not contain tag initially");

            _spec.AddTag(testTag);
            Assert.IsTrue(_spec.HasTag(testTag), "Should contain tag after adding");
        }

        [Test]
        public void HasAnyOrientationTag_ReturnsTrueIfPresent() {
            Assert.IsFalse(_spec.HasAnyOrientationTag(), "Should not have orientation tags by default");

            _spec.AddTag(TileSpec.ORIENTATION_EAST);
            Assert.IsTrue(_spec.HasAnyOrientationTag(), "Should detect East orientation tag");
        }

        [Test]
        public void DrawStyle_Overrides_BaseDrawProperties() {
            // Without tags, floor draws as floor
            Assert.IsTrue(_spec.DrawAdjacentWalls, "Floor should trigger adjacent walls draw by default");

            // Add wall override tag
            _spec.AddTag(TileSpec.DRAW_STYLE_WALL);
            _spec.RefreshCache();

            Assert.IsTrue(_spec.DrawWalls, "DRAW_STYLE_WALL should flag DrawWalls to true");
            Assert.IsFalse(_spec.DrawAsFloor, "DRAW_STYLE_WALL should make DrawAsFloor false for a normal floor tile");
        }
    }
}
