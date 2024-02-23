using System.Linq;
using TiledCS;
using UnityEngine;

namespace DungeonestCrab.Dungeon.TiledLoader {
    public abstract class TiledMapLoader : MonoBehaviour {

        protected TiledMap _map;
        protected TheDungeon _dungeon;

        protected virtual void InitialSetup(TheDungeon dungeon, TiledMap map) { }
        protected abstract void HandleObject(TheDungeon dungeon, TiledLayer layer, TileSpec tile, TiledObject obj);
        protected abstract void HandleTile(TheDungeon dungeon, TileSpec tile, Vector2Int tiledCoords);
        protected virtual void LoadMapProperties(TheDungeon dungeon, TiledMap map) { }
        protected virtual void MapFinishedLoading(TheDungeon dungeon, TiledMap map) { }

        public int LookupGIDWithUnityCoords(TiledLayer layer, Vector2Int pos, int tilesetOffset) {
            return LookupGIDInt(layer, ModifiedCoords(pos), tilesetOffset);
        }

        protected int LookupGID(TiledLayer layer, Vector2Int pos, int tilesetOffset) {
            return LookupGIDInt(layer, pos, tilesetOffset);
        }

        protected int GetFirstGidForTileset(string tilesetName) {
            TiledMapTileset tileset = _map.Tilesets.Where(x => x.source == tilesetName).FirstOrDefault();
            if (tileset != null) return tileset.firstgid;
            Debug.LogWarning($"No tileset named {tilesetName} found!");
            return 0;
        }

        int LookupGIDInt(TiledLayer layer, Vector2Int pos, int offset = 0) {
            int index = (pos.y * layer.width) + pos.x;
            return layer.data[index] - offset;
        }

        protected TiledLayer GetLayer(string name) {
            return _map.Layers.Where(x => x.name == name).FirstOrDefault();
        }

        public void LoadMap(TextAsset mapText) {
            if (mapText == null) {
                Debug.LogWarning("Cannot load map. It's null!");
            }
            LoadMap(mapText.text);
        }

        public void LoadMap(string mapString) {
            _map = new TiledMap();
            _map.ParseXml(mapString);

            _dungeon = new TheDungeon(_map.Width, _map.Height);
            InitialSetup(_dungeon, _map);

            foreach (TileSpec tile in _dungeon.AllTiles()) {
                Vector2Int modifiedCoords = ModifiedCoords(tile.Coords);
                HandleTile(_dungeon, tile, modifiedCoords);
            }

            foreach (TiledLayer objects in _map.Layers.Where(x => x.type == TiledLayerType.ObjectLayer)) {
                foreach (TiledObject obj in objects.objects) {
                    Vector2 correctedPos = new Vector2(obj.x, obj.y);
                    // For some reason, image-based objects have an origin at the
                    // bottom of their display, whereas other ones have it at the
                    // top.
                    if (obj.gid != 0) {
                        correctedPos.y -= obj.height;
                    }
                    Vector2 posf = new(correctedPos.x / _map.TileWidth, correctedPos.y / _map.TileHeight);
                    Vector2Int pos = Vector2Int.RoundToInt(posf);

                    HandleObject(_dungeon, objects, _dungeon.GetTileSpecSafe(pos), obj);
                }
            }

            LoadMapProperties(_dungeon, _map);
            MapFinishedLoading(_dungeon, _map);
        }

        public static float RotationForDir(string dir) {
            return dir switch {
                "E" => 90,
                "S" => 180,
                "W" => 270,
                _ => 0,
            };
        }

        public Vector2Int Dimensions {
            get => new Vector2Int(_map.Width, _map.Height);
        }

        /// <summary>
        /// Converts Unity coordinates into Tiled ones.
        /// </summary>
        protected Vector2Int ModifiedCoords(Vector2Int coords) {
            return new Vector2Int(coords.x, _map.Height - 1 - coords.y);
        }
    }
}