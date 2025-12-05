using System.Collections.Generic;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
    public class CarverStandard : ITileCarver {
        private readonly TerrainSO _terrain;
        private readonly Tile _tile;

        public CarverStandard(Tile tile, TerrainSO terrain = null) {
            _tile = tile;
            _terrain = terrain;
        }

        public void Carve(TheDungeon dungeon, IEnumerable<Vector2Int> path, IRandom rand) {
            foreach (var p in path) {
                TileSpec spec = dungeon.GetTileSpec(p);
                if (spec.Immutable) continue;

                spec.Tile = _tile;
                if (_terrain != null) {
                    spec.Terrain = _terrain;
                }
            }
        }
    }
}