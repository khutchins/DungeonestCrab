using DungeonestCrab.Dungeon.Generator;
using Ink.Parsed;
using Pomerandomian;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    public class TerrainSetter : IAlterer {

        readonly IMatcher _matcher;
        readonly int _range;
        readonly TerrainSO _terrainToSet;

        public TerrainSetter(IMatcher matcher, int range, TerrainSO terrainToSet) {
            _matcher = matcher;
            _range = range;
            _terrainToSet = terrainToSet;
        }

        public bool Modify(TheDungeon dungeon, IRandom rand) {
            var set = new HashSet<TileSpec>();
            foreach (TileSpec tile in dungeon.AllTiles()) {
                if (_matcher.Matches(tile)) {
                    for (int x = -_range; x <= _range; x++) {
                        for (int y = -_range; y <= _range; y++) {
                            var neighbor = dungeon.GetTileSpecSafe(tile.Coords.x + x, tile.Coords.y + y);
                            set.Add(neighbor);
                        }
                    }
                }
            }
            foreach (var tile in set) {
                tile.Terrain = _terrainToSet;
            }
            return true;
        }
    }
}