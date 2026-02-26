using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
    public class CarverDirectional : ITileCarver {
        private readonly TerrainSO _terrain;
        private readonly Tile _tile;
        private readonly bool _preserveExistingFloors;

        public CarverDirectional(Tile tile, TerrainSO terrain = null, bool preserveExistingFloors = false) {
            _tile = tile;
            _terrain = terrain;
            _preserveExistingFloors = preserveExistingFloors;
        }

        public void Carve(TheDungeon dungeon, IEnumerable<Vector2Int> path, IRandom rand) {
            var pathList = path.ToList();
            if (pathList.Count == 0) return;

            for (int i = 0; i < pathList.Count; i++) {
                Vector2Int current = pathList[i];
                TileSpec spec = dungeon.GetTileSpec(current);
                
                if (spec.Immutable) continue;
                if (_preserveExistingFloors && spec.Tile == Tile.Floor) continue;

                spec.Tile = _tile;
                if (_terrain != null) {
                    spec.Terrain = _terrain;
                }

                if (i < pathList.Count - 1) {
                    AddOrientationTag(spec, pathList[i + 1] - current);
                }
                if (i > 0) {
                    AddOrientationTag(spec, pathList[i - 1] - current);
                }
            }
        }

        private void AddOrientationTag(TileSpec spec, Vector2Int direction) {
            if (direction == Vector2Int.up) spec.AddTag(TileSpec.ORIENTATION_NORTH);
            else if (direction == Vector2Int.down) spec.AddTag(TileSpec.ORIENTATION_SOUTH);
            else if (direction == Vector2Int.right) spec.AddTag(TileSpec.ORIENTATION_EAST);
            else if (direction == Vector2Int.left) spec.AddTag(TileSpec.ORIENTATION_WEST);
        }
    }
}
