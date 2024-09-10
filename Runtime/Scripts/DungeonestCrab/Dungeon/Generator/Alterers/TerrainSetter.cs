using DungeonestCrab.Dungeon.Generator;
using Pomerandomian;
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
            foreach (TileSpec tile in dungeon.AllTiles()) {
                if (_matcher.Matches(tile)) {
                    for (int x = -_range; x <= _range; x++) {
                        for (int y = -_range; y <= _range; y++) {
                            var neighbor = dungeon.GetTileSpecSafe(tile.Coords.x + x, tile.Coords.y + y);
                            neighbor.Terrain = _terrainToSet;
                        }
                    }
                }
            }
            return true;
        }
    }
}