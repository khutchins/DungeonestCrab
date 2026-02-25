using System.Collections.Generic;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
    public class CarverStandard : ITileCarver {
        private readonly TerrainSO _terrain;
        private readonly Tile _tile;
        private readonly bool _preserveExistingFloors;

        public CarverStandard(Tile tile, TerrainSO terrain = null, bool preserveExistingFloors = false) {
            _tile = tile;
            _terrain = terrain;
            _preserveExistingFloors = preserveExistingFloors;
        }

        public void Carve(TheDungeon dungeon, IEnumerable<Vector2Int> path, IRandom rand) {
            foreach (var p in path) {
                TileSpec spec = dungeon.GetTileSpec(p);
                if (spec.Immutable) continue;
                if (_preserveExistingFloors && spec.Tile == Tile.Floor) continue;

                spec.Tile = _tile;
                if (_terrain != null) {
                    spec.Terrain = _terrain;
                }
            }
        }
    }
}