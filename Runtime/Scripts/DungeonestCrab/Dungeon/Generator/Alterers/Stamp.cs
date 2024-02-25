using DungeonestCrab;
using DungeonestCrab.Dungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
	public class Stamp {
		public readonly int xOffset;
		public readonly int yOffset;
		public readonly int W;
		public readonly int H;

		public Tile[,] Tiles;
		public TerrainSO[,] ExistingTerrain;
		public int[,] Styles;

		public Stamp(TheDungeon source, bool passTerrains, AppliedBounds bounds) {
			xOffset = bounds.x;
			yOffset = bounds.y;
			W = bounds.w;
			H = bounds.h;

			Tiles = new Tile[bounds.h, bounds.w];
			ExistingTerrain = new TerrainSO[bounds.h, bounds.w];
			Styles = new int[bounds.h, bounds.w];
			for (int iy = 0; iy < bounds.h; iy++) {
				for (int ix = 0; ix < bounds.w; ix++) {
					Tiles[iy, ix] = Tile.Unset;
					ExistingTerrain[iy, ix] = passTerrains ? source.GetTileSpec(ix + bounds.x, iy + bounds.y).Terrain : null;
					Styles[iy, ix] = (int)PaintStyle.Default;
				}
			}
		}

		public Stamp(int w, int h) : this(0, 0, w, h) { }

		public Stamp(int x, int y, int w, int h) {
			xOffset = x;
			yOffset = y;
			W = w;
			H = h;

			Tiles = new Tile[h, w];
			ExistingTerrain = new TerrainSO[h, w];
			Styles = new int[h, w];
			for (int iy = 0; iy < h; iy++) {
				for (int ix = 0; ix < w; ix++) {
					Tiles[iy, ix] = Tile.Unset;
					ExistingTerrain[iy, ix] = null;
					Styles[iy, ix] = (int)PaintStyle.Default;
				}
			}
		}

		public void GetStamped(Stamp otherStamp) {
			for (int iy = 0; iy < otherStamp.H && iy + otherStamp.yOffset < this.H; iy++) {
				for (int ix = 0; ix <otherStamp.W && ix + otherStamp.xOffset < this.W; ix++) {
					if (otherStamp.Tiles[iy, ix] != Tile.Unset) {
						MaybeSetAt(ix + otherStamp.xOffset, iy + otherStamp.yOffset, otherStamp.At(ix, iy), otherStamp.StyleAt(ix, iy));
					}
				}
			}
		}

		public bool MaybeSetAt(Vector2Int pt, Tile tile, int style = (int)PaintStyle.Default) {
			return MaybeSetAt(pt.x, pt.y, tile, style);
		}

		public bool MaybeSetAt(DirectedPoint pt, Tile tile, int style = (int)PaintStyle.Default) {
			return MaybeSetAt(pt.x, pt.y, tile, style);
		}

		public bool MaybeSetAt(int x, int y, Tile tile, int style = (int)PaintStyle.Default) {
			if (!CanSetAt(x, y)) return false;

			Tiles[y, x] = tile;
			Styles[y, x] = style;
			return true;
		}

		public int StyleAt(DirectedPoint pt) {
			return Styles[pt.y, pt.x];
		}

		public int StyleAt(int x, int y) {
			return Styles[y, x];
		}

		public void MaybeSetDirectional(DirectedPoint pt, Tile tile, Dir dir) {
			MaybeSetDirectional(pt.x, pt.y, tile, dir);
		}

		public void MaybeSetDirectional(int x, int y, Tile tile, Dir dir) {
			int style = StyleAt(x, y);
			int newPaintDir = (dir == Dir.Up || dir == Dir.Down) ? (int)PaintStyle.FloorNS : (int)PaintStyle.FloorEW;
			int computedStyle = (int)PaintStyle.Default;
			if (style == (int)PaintStyle.Default) {
				computedStyle = newPaintDir;
			} else if (style == (int)PaintStyle.FloorEW || style == (int)PaintStyle.FloorNS) {
				if (newPaintDir != style) {
					computedStyle = (int)PaintStyle.FloorAllDirs;
				} else {
					computedStyle = newPaintDir;
				}
			}
			MaybeSetAt(x, y, tile, computedStyle);
		}

		public bool CanSetAt(DirectedPoint pt) {
			return CanSetAt(pt.x, pt.y);
		}

		public bool CanSetAt(Vector2Int pt) {
			return CanSetAt(pt.x, pt.y);
		}

		public bool CanSetAt(int x, int y) {
			return x >= 0 && y >= 0 && x < W && y < H && ExistingTerrain[y, x] == null;
		}

		public Tile At(int x, int y) {
			return Tiles[y, x];
		}

		public Tile At(int x, int y, Tile defaultValue) {
			if (x < 0 || y < 0 || x >= W || y >= H) return defaultValue;
			return Tiles[y, x];
		}

		public Tile At(DirectedPoint pt) {
			return Tiles[pt.y, pt.x];
		}

		public Tile At(Vector2Int pt) {
			return Tiles[pt.y, pt.x];
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
