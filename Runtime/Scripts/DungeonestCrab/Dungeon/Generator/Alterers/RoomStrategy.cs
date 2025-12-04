using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {

    [Serializable]
    public abstract class RoomStrategy {
        public int MinX = 3;
        public int MaxX = 5;
        public int MinY = 3;
        public int MaxY = 5;

        [Tooltip("Relative probability of picking this room type.")]
        public int Weight = 1;

        public abstract void StampRoom(Stamp stamp, IRandom rand);

        protected void Set(Stamp stamp, int x, int y, Tile tile, params string[] tags) {
            stamp.MaybeSetAt(x, y, tile, tags);
        }
    }

    [Serializable]
    public class RoomBasic : RoomStrategy {
        public override void StampRoom(Stamp stamp, IRandom rand) {
            for (int y = 0; y < stamp.H; y++) {
                for (int x = 0; x < stamp.W; x++) {
                    if (y == 0 || x == 0 || y == stamp.H - 1 || x == stamp.W - 1) {
                        Set(stamp, x, y, Tile.Wall);
                    } else {
                        Set(stamp, x, y, Tile.Floor);
                    }
                }
            }
        }
    }

    [Serializable]
    public class RoomRounded : RoomStrategy {
        public override void StampRoom(Stamp stamp, IRandom rand) {
            int w = stamp.W - 2;
            int h = stamp.H - 2;
            float radW = ((w - 2) / 2F);
            float radH = ((h - 2) / 2F);
            float radW2 = radW * radW;
            float radH2 = radH * radH;

            for (int y = 1; y < stamp.H - 1; y++) {
                for (int x = 1; x < stamp.W - 1; x++) {
                    float dx = (stamp.W / 2F) - x;
                    float dy = (stamp.H / 2F) - y;

                    // Ellipse check
                    if (dx * dx / radW2 + dy * dy / radH2 <= 1) {
                        Set(stamp, x, y, Tile.Floor);
                    }
                }
            }
            RoomSpecHelpers.Outline(stamp);
        }
    }

    [Serializable]
    public class RoomRoundedCorners : RoomStrategy {
        public override void StampRoom(Stamp stamp, IRandom rand) {
            for (int y = 0; y < stamp.H; y++) {
                for (int x = 0; x < stamp.W; x++) {
                    bool edge = y == 0 || x == 0 || y == stamp.H - 1 || x == stamp.W - 1;
                    bool corner = (x == 1 || x == stamp.W - 2) && (y == 1 || y == stamp.H - 2);

                    if (edge || corner) {
                        Set(stamp, x, y, Tile.Wall);
                    } else {
                        Set(stamp, x, y, Tile.Floor);
                    }
                }
            }
        }
    }

    [Serializable]
    public class RoomLibrary : RoomStrategy {
        public override void StampRoom(Stamp stamp, IRandom rand) {
            bool vert = stamp.H >= stamp.W;

            if (vert) {
                for (int y = 0; y < stamp.H; y++) {
                    for (int x = 0; x < stamp.W; x++) {
                        if (x == 0 || y == 0 || x == stamp.W - 1 || y == stamp.H - 1) {
                            Set(stamp, x, y, Tile.Wall);
                        } else if (y % 2 == 0 && Mathf.Abs((stamp.W - 1) / 2F - x) >= 1) {
                            Set(stamp, x, y, Tile.Wall); // Interior Wall
                        } else if (y % 2 == 1) {
                            Set(stamp, x, y, Tile.Floor, "style:bookcase");
                        } else {
                            Set(stamp, x, y, Tile.Floor, "style:bookcase_end");
                        }
                    }
                }
            } else {
                for (int y = 0; y < stamp.H; y++) {
                    for (int x = 0; x < stamp.W; x++) {
                        if (x == 0 || y == 0 || x == stamp.W - 1 || y == stamp.H - 1) {
                            Set(stamp, x, y, Tile.Wall);
                        } else if (x % 2 == 0 && Mathf.Abs((stamp.H - 1) / 2F - y) >= 1) {
                            Set(stamp, x, y, Tile.Wall);
                        } else if (x % 2 == 1) {
                            Set(stamp, x, y, Tile.Floor, "style:bookcase");
                        } else {
                            Set(stamp, x, y, Tile.Floor, "style:bookcase_end");
                        }
                    }
                }
            }
        }
    }

    [Serializable]
    public class RoomBaths : RoomStrategy {
        public override void StampRoom(Stamp stamp, IRandom rand) {
            for (int y = 0; y < stamp.H; y++) {
                for (int x = 0; x < stamp.W; x++) {
                    bool edge = y == 0 || x == 0 || y == stamp.H - 1 || x == stamp.W - 1;
                    bool walkway = y == 1 || x == 1 || y == stamp.H - 2 || x == stamp.W - 2;

                    if (edge) {
                        Set(stamp, x, y, Tile.Wall);
                    } else if (walkway) {
                        Set(stamp, x, y, Tile.Floor);
                    } else {
                        Set(stamp, x, y, Tile.Wall, "style:sunken_flooded", TileSpec.DRAW_STYLE_FLOOR);
                    }
                }
            }
        }
    }

    [Serializable]
    public class RoomBlob : RoomStrategy {
        [Range(0, 1)] public float InitialOdds = 0.5f;
        public int Iterations = 5;
        public int Neighbors = 5;

        public override void StampRoom(Stamp stamp, IRandom rand) {
            for (int iy = 1; iy < stamp.H - 1; iy++) {
                for (int ix = 1; ix < stamp.W - 1; ix++) {
                    if (rand.WithPercentChance(InitialOdds)) {
                        Set(stamp, ix, iy, Tile.Floor);
                    }
                }
            }

            for (int i = 0; i < Iterations; i++) {
                List<Vector2Int> toFloor = new List<Vector2Int>();
                List<Vector2Int> toVoid = new List<Vector2Int>();

                for (int iy = 1; iy < stamp.H - 1; iy++) {
                    for (int ix = 1; ix < stamp.W - 1; ix++) {
                        int walls = 0;
                        for (int ny = -1; ny <= 1; ny++)
                            for (int nx = -1; nx <= 1; nx++)
                                if (nx != 0 || ny != 0) {
                                    if (stamp.At(ix + nx, iy + ny) == Tile.Unset) walls++;
                                }

                        if (walls >= Neighbors) toVoid.Add(new Vector2Int(ix, iy));
                        else toFloor.Add(new Vector2Int(ix, iy));
                    }
                }

                foreach (var p in toFloor) Set(stamp, p.x, p.y, Tile.Floor);
                foreach (var p in toVoid) Set(stamp, p.x, p.y, Tile.Unset);
            }

            RoomSpecHelpers.Outline(stamp);
        }
    }
}