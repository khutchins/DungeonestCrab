using UnityEngine;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Printer {

    public struct TileRuleConfig {
        public bool DrawCeiling;
        public float WallHeight;
        public float CeilingHeight;
        public float GroundOffset;

        public TileRuleConfig(TileSpec spec) {
            DrawCeiling = spec.HasCeiling;
            WallHeight = spec.Terrain.WallHeight;
            CeilingHeight = spec.CeilingOffset;
            GroundOffset = spec.GroundOffset;
        }
    }

    public struct WallStyleConfig {
        public TileSpec StyleSource;
        public bool SuppressWall;

        public WallStyleConfig(TileSpec defaultSource) {
            StyleSource = defaultSource;
            SuppressWall = false;
        }
    }
}