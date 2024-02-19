using DungeonestCrab.Dungeon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    public interface IMatcher {
        bool Matches(TileSpec tile);
    }

    public class OrMatcher : IMatcher {
        private readonly IMatcher[] _matchers;

        public OrMatcher(params IMatcher[] matchers) {
            _matchers = matchers;
        }

        public bool Matches(TileSpec tile) {
            return _matchers.Where(x => x.Matches(tile)).Any();
        }
    }

    public class TileMatcher : IMatcher {
        private readonly Tile? _tile;
        private readonly bool _anyTile;
        private readonly TerrainSO _terrain;
        private readonly bool _anyTerrain;
        private readonly int? _style;
        private readonly bool _anyStyle;

        public TileMatcher(Tile tile, bool anyTile, TerrainSO terrain, bool anyTerrain, int style, bool anyStyle) {
            _tile = tile;
            _anyTile = anyTile;
            _terrain = terrain;
            _anyTerrain = anyTerrain;
            _style = style;
            _anyStyle = anyStyle;
        }

        public bool Matches(TileSpec tile) {
            return (_anyTile || tile.Tile == _tile)
                && (_anyTerrain || tile.Terrain == _terrain)
                && (_anyStyle || tile.Style == _style);
        }

        public static TileMatcher MatchingAll() {
            return new TileMatcher(Tile.Unset, true, null, true, 0, true);
        }

        public static TileMatcher Matching(Tile tile) {
            return new TileMatcher(tile, false, null, true, 0, true);
        }

        public static TileMatcher Matching(TerrainSO terrain) {
            return new TileMatcher(Tile.Unset, true, terrain, false, 0, true);
        }

        public static TileMatcher Matching(int style) {
            return new TileMatcher(Tile.Unset, true, null, true, style, false);
        }

        public static TileMatcher Matching(Tile tile, TerrainSO terrain) {
            return new TileMatcher(tile, false, terrain, false, 0, true);
        }

        public static TileMatcher Matching(TerrainSO terrain, Tile tile) {
            return new TileMatcher(tile, false, terrain, false, 0, true);
        }
    }
}