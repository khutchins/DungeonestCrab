using NUnit.Framework;
using DungeonestCrab.Dungeon.Printer;
using System.Collections.Generic;
using System.Linq;

namespace DungeonestCrab.Tests {
    public class DungeonPrinterWallTests {

        private static TileRuleConfig MakeRules(float wallHeight = 1, float ceilingHeight = 1, float groundOffset = 0) {
            return new TileRuleConfig {
                WallHeight = wallHeight,
                CeilingHeight = ceilingHeight,
                GroundOffset = groundOffset,
                DrawCeiling = true,
            };
        }

        private static void AssertSegment(
                DungeonPrinter.WallSegment seg,
                DungeonPrinter.WallSegmentType expectedType,
                float expectedMinY, float expectedMaxY,
                float tolerance = 0.001f) {
            Assert.AreEqual(expectedType, seg.Type, $"Segment type mismatch");
            Assert.AreEqual(expectedMinY, seg.MinY, tolerance, $"MinY mismatch for {seg.Type}");
            Assert.AreEqual(expectedMaxY, seg.MaxY, tolerance, $"MaxY mismatch for {seg.Type}");
        }

        // Standard wall tests (floor looking at wall)

        [Test]
        public void SameHeight_FloorToWall_OneStandardSegment() {
            var my = MakeRules(wallHeight: 1, ceilingHeight: 1);
            var adj = MakeRules(wallHeight: 1, ceilingHeight: 1);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: true, adjDrawsAsFloor: false, tileHeightMult: 1);

            Assert.AreEqual(1, segs.Count, "Should have exactly 1 segment");
            AssertSegment(segs[0], DungeonPrinter.WallSegmentType.Standard, 0, 1);
        }

        [Test]
        public void TallCeiling_ShortWall_StandardPlusUpper() {
            var my = MakeRules(wallHeight: 1, ceilingHeight: 3);
            var adj = MakeRules(wallHeight: 1, ceilingHeight: 1);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: true, adjDrawsAsFloor: false, tileHeightMult: 1);

            Assert.AreEqual(2, segs.Count, "Should have standard + upper");
            AssertSegment(segs[0], DungeonPrinter.WallSegmentType.Standard, 0, 1);
            AssertSegment(segs[1], DungeonPrinter.WallSegmentType.Upper, 1, 3);
        }

        [Test]
        public void StyleSourceShorterThanNeighborCeiling_UpperStartsAtCeiling() {
            var my = MakeRules(wallHeight: 1, ceilingHeight: 3);
            var adj = MakeRules(wallHeight: 2, ceilingHeight: 2);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: true, adjDrawsAsFloor: false, tileHeightMult: 1);

            Assert.AreEqual(2, segs.Count, "Should have standard + upper");
            AssertSegment(segs[0], DungeonPrinter.WallSegmentType.Standard, 0, 1);
            AssertSegment(segs[1], DungeonPrinter.WallSegmentType.Upper, 2, 3);
        }

        [Test]
        public void StyleWallTallerThanNeighborCeiling_UpperStartsAtStandardWallTop() {
            var my = MakeRules(wallHeight: 2, ceilingHeight: 3);
            var adj = MakeRules(wallHeight: 1, ceilingHeight: 1);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 2, drawsStandardWalls: true, adjDrawsAsFloor: false, tileHeightMult: 1);

            Assert.AreEqual(2, segs.Count, "Should have standard + upper");
            AssertSegment(segs[0], DungeonPrinter.WallSegmentType.Standard, 0, 2);
            AssertSegment(segs[1], DungeonPrinter.WallSegmentType.Upper, 2, 3);
        }

        // Cliff terrain regression (floor -> wall, same ceiling)

        [Test]
        public void SameCeiling_StandardWalls_NoUpper() {
            var my = MakeRules(wallHeight: 1, ceilingHeight: 2);
            var adj = MakeRules(wallHeight: 1, ceilingHeight: 2);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: true, adjDrawsAsFloor: false, tileHeightMult: 1);

            Assert.AreEqual(1, segs.Count, "Same ceiling: only standard wall, no upper");
            AssertSegment(segs[0], DungeonPrinter.WallSegmentType.Standard, 0, 1);
        }

        // Wall-to-wall (no standard walls, adj is NOT floor)
        // The SHORT tile draws the upper wall so it faces the short side.

        [Test]
        public void TwoWalls_SameWallHeight_NoSegments() {
            var my = MakeRules(wallHeight: 1, ceilingHeight: 1);
            var adj = MakeRules(wallHeight: 1, ceilingHeight: 1);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: false, adjDrawsAsFloor: false, tileHeightMult: 1);

            Assert.AreEqual(0, segs.Count, "Same wall height: no segments");
        }

        [Test]
        public void TwoWalls_SameWallHeight_DifferentCeiling_NoSegments() {
            var my = MakeRules(wallHeight: 1, ceilingHeight: 2);
            var adj = MakeRules(wallHeight: 1, ceilingHeight: 3);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: false, adjDrawsAsFloor: false, tileHeightMult: 1);

            Assert.AreEqual(0, segs.Count, "Same wall height: no segments even with different ceilings");
        }

        [Test]
        public void TwoWalls_ShortLookingAtTall_DrawsUpper() {
            // Short wall (WH=1) looking at tall wall (WH=2). Draws upper 1 -> 2 facing the short side.
            var my = MakeRules(wallHeight: 1, ceilingHeight: 1);
            var adj = MakeRules(wallHeight: 2, ceilingHeight: 2);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: false, adjDrawsAsFloor: false, tileHeightMult: 1);

            Assert.AreEqual(1, segs.Count, "Short tile should draw upper wall facing itself");
            AssertSegment(segs[0], DungeonPrinter.WallSegmentType.Upper, 1, 2);
        }

        [Test]
        public void TwoWalls_TallLookingAtShort_NoSegments() {
            // Tall wall (WH=2) looking at short wall (WH=1). Does NOT draw (short side draws it).
            var my = MakeRules(wallHeight: 2, ceilingHeight: 2);
            var adj = MakeRules(wallHeight: 1, ceilingHeight: 1);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 2, drawsStandardWalls: false, adjDrawsAsFloor: false, tileHeightMult: 1);

            Assert.AreEqual(0, segs.Count, "Tall tile should NOT draw upper wall (short side draws it)");
        }

        [Test]
        public void TwoWalls_SameCeiling_ShortLookingAtTall_DrawsUpper() {
            // Wall(WH=1, CH=2) looking at Wall(WH=2, CH=2). Same ceiling, different wall heights.
            var my = MakeRules(wallHeight: 1, ceilingHeight: 2);
            var adj = MakeRules(wallHeight: 2, ceilingHeight: 2);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: false, adjDrawsAsFloor: false, tileHeightMult: 1);

            Assert.AreEqual(1, segs.Count, "Short tile should draw upper wall facing itself");
            AssertSegment(segs[0], DungeonPrinter.WallSegmentType.Upper, 1, 2);
        }

        [Test]
        public void TwoWalls_DifferentWallAndCeiling_UpperUsesWallHeightOnly() {
            // Wall(WH=1, CH=1) looking at Wall(WH=2, CH=3). Upper goes to adjWH=2, not adjCH=3.
            var my = MakeRules(wallHeight: 1, ceilingHeight: 1);
            var adj = MakeRules(wallHeight: 2, ceilingHeight: 3);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: false, adjDrawsAsFloor: false, tileHeightMult: 1);

            Assert.AreEqual(1, segs.Count, "Should bridge wall height gap only");
            AssertSegment(segs[0], DungeonPrinter.WallSegmentType.Upper, 1, 2);
        }

        // Floor-to-floor (no standard walls, adj IS floor)

        [Test]
        public void TwoFloors_DifferentCeilings_UpperBridgesGap() {
            var my = MakeRules(wallHeight: 1, ceilingHeight: 3);
            var adj = MakeRules(wallHeight: 1, ceilingHeight: 1);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: false, adjDrawsAsFloor: true, tileHeightMult: 1);

            Assert.AreEqual(1, segs.Count, "Should have 1 upper segment");
            AssertSegment(segs[0], DungeonPrinter.WallSegmentType.Upper, 1, 3);
        }

        [Test]
        public void TwoFloors_SameCeiling_NoSegments() {
            var my = MakeRules(wallHeight: 1, ceilingHeight: 2);
            var adj = MakeRules(wallHeight: 1, ceilingHeight: 2);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: false, adjDrawsAsFloor: true, tileHeightMult: 1);

            Assert.AreEqual(0, segs.Count, "Same ceilings should produce no segments");
        }

        // Lower wall tests

        [Test]
        public void SunkenGround_LowerSegment() {
            var my = MakeRules(wallHeight: 1, ceilingHeight: 1, groundOffset: 2);
            var adj = MakeRules(wallHeight: 1, ceilingHeight: 1, groundOffset: 0);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: true, adjDrawsAsFloor: false, tileHeightMult: 1);

            Assert.IsTrue(segs.Any(s => s.Type == DungeonPrinter.WallSegmentType.Lower), "Should have a lower segment");
            var lower = segs.First(s => s.Type == DungeonPrinter.WallSegmentType.Lower);
            AssertSegment(lower, DungeonPrinter.WallSegmentType.Lower, -2, 0);
        }

        [Test]
        public void NeighborDeeperGround_NoLowerSegment() {
            var my = MakeRules(wallHeight: 1, ceilingHeight: 1, groundOffset: 0);
            var adj = MakeRules(wallHeight: 1, ceilingHeight: 1, groundOffset: 3);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: true, adjDrawsAsFloor: false, tileHeightMult: 1);

            Assert.IsFalse(segs.Any(s => s.Type == DungeonPrinter.WallSegmentType.Lower), "Should NOT have a lower segment");
        }

        [Test]
        public void SunkenGround_WithStandard_BothSegments() {
            var my = MakeRules(wallHeight: 1, ceilingHeight: 1, groundOffset: 2);
            var adj = MakeRules(wallHeight: 1, ceilingHeight: 1, groundOffset: 0);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: true, adjDrawsAsFloor: false, tileHeightMult: 1);

            Assert.AreEqual(2, segs.Count, "Should have lower + standard");
            AssertSegment(segs[0], DungeonPrinter.WallSegmentType.Lower, -2, 0);
            AssertSegment(segs[1], DungeonPrinter.WallSegmentType.Standard, 0, 1);
        }

        // Tile height multiplier

        [Test]
        public void TileHeightMult_ScalesAllHeights() {
            var my = MakeRules(wallHeight: 1, ceilingHeight: 2);
            var adj = MakeRules(wallHeight: 1, ceilingHeight: 1);

            var segs = DungeonPrinter.ComputeWallSegments(my, adj, styleWallHeight: 1, drawsStandardWalls: true, adjDrawsAsFloor: false, tileHeightMult: 2);

            Assert.AreEqual(2, segs.Count, "Should have standard + upper");
            AssertSegment(segs[0], DungeonPrinter.WallSegmentType.Standard, 0, 2);
            AssertSegment(segs[1], DungeonPrinter.WallSegmentType.Upper, 2, 4);
        }

        // Validity checks

        [Test]
        public void AllSegments_HavePositiveHeight() {
            float[] heights = { 0.5f, 1, 1.5f, 2, 3 };
            float[] offsets = { 0, 1, 2 };

            foreach (float myWH in heights)
            foreach (float myCH in heights)
            foreach (float adjWH in heights)
            foreach (float adjCH in heights)
            foreach (float myGO in offsets)
            foreach (float adjGO in offsets)
            foreach (bool stdWalls in new[] { true, false })
            foreach (bool adjFloor in new[] { true, false }) {
                var my = MakeRules(myWH, myCH, myGO);
                var adj = MakeRules(adjWH, adjCH, adjGO);
                var segs = DungeonPrinter.ComputeWallSegments(my, adj, myWH, stdWalls, adjFloor, 1);

                foreach (var seg in segs) {
                    Assert.Less(seg.MinY, seg.MaxY,
                        $"Invalid segment {seg.Type}: minY={seg.MinY} maxY={seg.MaxY} " +
                        $"(myWH={myWH} myCH={myCH} adjWH={adjWH} adjCH={adjCH} myGO={myGO} adjGO={adjGO} std={stdWalls} floor={adjFloor})");
                }
            }
        }
    }
}
