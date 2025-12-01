using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
	public class Stamp {
		public readonly int xOffset;
		public readonly int yOffset;
		public readonly int W;
		public readonly int H;

		public readonly TileSpec[,] Tiles;
        public readonly AppliedBounds Bounds;

		public Stamp(TheDungeon source, bool passTerrains, AppliedBounds bounds) {
			xOffset = bounds.x;
			yOffset = bounds.y;
			W = bounds.w;
			H = bounds.h;

            Tiles = new TileSpec[H, W];
            for (int iy = 0; iy < bounds.h; iy++) {
				for (int ix = 0; ix < bounds.w; ix++) {
					Tiles[iy, ix] = new TileSpec(new Vector2Int(iy, ix), Tile.Unset, passTerrains ? source.GetTileSpec(ix + bounds.x, iy + bounds.y).Terrain : null, false);
				}
			}
		}

		public Stamp(int w, int h) : this(0, 0, w, h) { }

		public Stamp(int x, int y, int w, int h) {
			xOffset = x;
			yOffset = y;
			W = w;
			H = h;

            Tiles = new TileSpec[h, w];
			for (int iy = 0; iy < h; iy++) {
				for (int ix = 0; ix < w; ix++) {
                    Tiles[iy, ix] = new TileSpec(new Vector2Int(ix, iy), Tile.Unset, null, false);
                }
			}
		}

		public void GetStamped(Stamp otherStamp) {
			for (int iy = 0; iy < otherStamp.H && iy + otherStamp.yOffset < this.H; iy++) {
				for (int ix = 0; ix < otherStamp.W && ix + otherStamp.xOffset < this.W; ix++) {
					if (otherStamp.Tiles[iy, ix].Tile != Tile.Unset) {
						MaybeSetAt(ix + otherStamp.xOffset, iy + otherStamp.yOffset, otherStamp.At(ix, iy), otherStamp.StylesAt(ix, iy));
					}
				}
			}
		}

		public bool MaybeSetAt(Vector2Int pt, Tile tile, params string[] styles) {
			return MaybeSetAt(pt.x, pt.y, tile, styles);
		}

		public bool MaybeSetAt(DirectedPoint pt, Tile tile, params string[] styles) {
			return MaybeSetAt(pt.x, pt.y, tile, styles);
		}

        public bool MaybeSetAt(int x, int y, Tile tile, params string[] styles) {
            if (!CanSetAt(x, y)) return false;

            var tileSpec = Tiles[y, x];
            tileSpec.Tile = tile;
            tileSpec.AddTag(styles);
            return true;
        }

        public bool MaybeSetAt(int x, int y, Tile tile, IEnumerable<string> styles) {
			if (!CanSetAt(x, y)) return false;

			var tileSpec = Tiles[y, x];
			tileSpec.Tile = tile;
			tileSpec.AddTags(styles);
			return true;
		}

        public IEnumerable<string> StylesAt(Vector2Int pt) {
			return StylesAt(pt.x, pt.y);
        }

        public IEnumerable<string> StylesAt(DirectedPoint pt) {
			return StylesAt(pt.x, pt.y);
		}

        public IEnumerable<string> StylesAt(int x, int y) {
			return Tiles[y, x].GetTags();
		}

		public void MaybeSetDirectional(DirectedPoint pt, Tile tile, Dir dir) {
			MaybeSetDirectional(pt.x, pt.y, tile, dir);
		}

		public void MaybeSetDirectional(int x, int y, Tile tile, Dir dir) {
			List<string> tags = new List<string>();
			if (dir == Dir.Up || dir == Dir.Down) {
				tags.Add(TileSpec.ORIENTATION_NORTH);
				tags.Add(TileSpec.ORIENTATION_SOUTH);
			} else if (dir == Dir.Left || dir == Dir.Right) {
				tags.Add(TileSpec.ORIENTATION_WEST);
                tags.Add(TileSpec.ORIENTATION_EAST);
            }
			MaybeSetAt(x, y, tile, tags);
		}

		public bool CanSetAt(DirectedPoint pt) {
			return CanSetAt(pt.x, pt.y);
		}

		public bool CanSetAt(Vector2Int pt) {
			return CanSetAt(pt.x, pt.y);
		}

		public bool CanSetAt(int x, int y) {
			return x >= 0 && y >= 0 && x < W && y < H && Tiles[y, x].Terrain == null;
		}

		public Tile At(int x, int y) {
			return Tiles[y, x].Tile;
		}

		public Tile At(int x, int y, Tile defaultValue) {
			if (x < 0 || y < 0 || x >= W || y >= H) return defaultValue;
			return Tiles[y, x].Tile;
		}

		public Tile At(DirectedPoint pt) {
			return Tiles[pt.y, pt.x].Tile;
		}

		public Tile At(Vector2Int pt) {
			return Tiles[pt.y, pt.x].Tile;
		}

		public Tile At(Vector2Int pt, Tile defaultValue) {
			return At(pt.x, pt.y, defaultValue);
		}

		public bool InBounds(Vector2Int pt) {
			return pt.x >= 0 && pt.y >= 0 && pt.x < W && pt.y < H;
		}

		/// <summary>
		/// An enumerable that iterates all coords in the stamp.
		/// </summary>
		public IEnumerable<Vector2Int> All() {
			for (int iy = 0; iy < H; iy++) {
				for (int ix = 0; ix < W; ix++) {
					yield return new Vector2Int(ix, iy);
				}
			}
		}

		public void StampCircle(Tile tile, Vector2Int c, float rad) {
			SourceCircle.StampCircle(this, tile, c.x, c.y, rad);
		}

		public void StampLine(Tile tile, Vector2Int from, Vector2Int to) {
			foreach (Vector2Int coord in from.Line(to)) {
				MaybeSetAt(coord, tile);
			}
		}
	}
}
