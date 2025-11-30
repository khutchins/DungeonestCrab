using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
    public class Morphology : IAlterer {
        public enum Operation { Dilation, Erosion, Open, Close }
        private readonly Operation _op;
        private readonly int _iterations = 1;

        public Morphology(Operation op, int iterations) {
            _op = op;
            _iterations = iterations;
        }

        public bool Modify(TheDungeon dungeon, IRandom rand) {
            for (int i = 0; i < _iterations; i++) {
                switch (_op) {
                    case Operation.Dilation: Dilate(dungeon); break;
                    case Operation.Erosion: Erode(dungeon); break;
                    case Operation.Open: Erode(dungeon); Dilate(dungeon); break;
                    case Operation.Close: Dilate(dungeon); Erode(dungeon); break;
                }
            }
            return true;
        }

        private void Dilate(TheDungeon d) {
            var toWall = new List<Vector2Int>();
            foreach (var tile in d.AllTiles()) {
                if (tile.Tile == Tile.Floor) {
                    // If any neighbor is a wall, become a wall
                    bool nearWall = false;
                    foreach (var adj in d.TileAdjacencies(tile.Coords)) {
                        if (adj.Tile == Tile.Wall) { nearWall = true; break; }
                    }
                    if (nearWall) toWall.Add(tile.Coords);
                }
            }
            foreach (var p in toWall) d.GetTileSpec(p).Tile = Tile.Wall;
        }

        private void Erode(TheDungeon d) {
            var toFloor = new List<Vector2Int>();
            foreach (var tile in d.AllTiles()) {
                if (tile.Tile == Tile.Wall) {
                    // If any neighbor is floor, become floor
                    bool nearFloor = false;
                    foreach (var adj in d.TileAdjacencies(tile.Coords)) {
                        if (adj.Tile == Tile.Floor) { nearFloor = true; break; }
                    }
                    if (nearFloor) toFloor.Add(tile.Coords);
                }
            }
            foreach (var p in toFloor) d.GetTileSpec(p).Tile = Tile.Floor;
        }
    }
}