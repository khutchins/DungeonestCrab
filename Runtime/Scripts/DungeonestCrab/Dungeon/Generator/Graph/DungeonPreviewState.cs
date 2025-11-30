using System.Collections.Generic;
using UnityEngine;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    public class DungeonPreviewState {
        public int Width;
        public int Height;
        public Tile[,] Tiles;
        public List<Entity> Entities;

        public DungeonPreviewState(int w, int h) {
            Width = w;
            Height = h;
            Tiles = new Tile[h, w];
            Entities = new List<Entity>();

            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    Tiles[y, x] = Tile.Unset;
                }
            }
        }

        public DungeonPreviewState Clone() {
            var clone = new DungeonPreviewState(Width, Height);
            System.Array.Copy(Tiles, clone.Tiles, Tiles.Length);
            clone.Entities.AddRange(Entities);
            return clone;
        }
    }
}