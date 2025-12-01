using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator {
	public interface IRoomSpec {
		int MinX { get; }
		int MinY { get; }
		int MaxX { get; }
		int MaxY { get; }
		
		void StampRoom(Stamp stamp, IRandom rand);
	}

	public class RoomSpecBasic : IRoomSpec {
		
		public RoomSpecBasic(int min, int max) {
			MinX = min;
			MinY = min;
			MaxX = max;
			MaxY = max;
		}

		public int MinX { get; set; }
		public int MinY { get; set; }
		public int MaxX { get; set; }
		public int MaxY { get; set; }

		public void StampRoom(Stamp stamp, IRandom rand) {
			for (int y = 0; y < stamp.H; y++) {
				for (int x = 0; x < stamp.W; x++) {
					if (y == 0 || x == 0 || y == stamp.H - 1 || x == stamp.W - 1) {
						stamp.MaybeSetAt(x, y, Tile.Wall);
					} else {
						stamp.MaybeSetAt(x, y, Tile.Floor);
					}
				}
			}
		}
	}

	public class RoomSpecBaths : IRoomSpec {

		public RoomSpecBaths(int min, int max) {
			MinX = min;
			MinY = min;
			MaxX = max;
			MaxY = max;
		}

		public int MinX { get; set; }
		public int MinY { get; set; }
		public int MaxX { get; set; }
		public int MaxY { get; set; }

		public void StampRoom(Stamp stamp, IRandom rand) {
			for (int y = 0; y < stamp.H; y++) {
				for (int x = 0; x < stamp.W; x++) {
					if (y == 0 || x == 0 || y == stamp.H - 1 || x == stamp.W - 1) {
						stamp.MaybeSetAt(x, y, Tile.Wall);
					} else if (y == 1 || x == 1 || y == stamp.H - 2 || x == stamp.W - 2) {
						stamp.MaybeSetAt(x, y, Tile.Floor);
					} else {
						stamp.MaybeSetAt(x, y, Tile.Wall, "style:sunkenflooded");
					}
				}
			}
		}
	}

	public class RoomSpecLibrary : IRoomSpec {

		public RoomSpecLibrary(int min, int max) {
			MinX = min;
			MinY = min;
			MaxX = max;
			MaxY = max;
		}

		public int MinX { get; set; }
		public int MinY { get; set; }
		public int MaxX { get; set; }
		public int MaxY { get; set; }

		public void StampRoom(Stamp stamp, IRandom rand) {
			bool vert = stamp.H >= stamp.W;
			if (vert) {
				for (int y = 0; y < stamp.H; y++) {
					for (int x = 0; x < stamp.W; x++) {
						if (x == 0 || y == 0 || x == stamp.W - 1 || y == stamp.H - 1) {
							stamp.MaybeSetAt(x, y, Tile.Wall);
						} else if (y % 2 == 0 && Mathf.Abs((stamp.W - 1) / 2F - x) >= 1) {
							stamp.MaybeSetAt(x, y, Tile.Wall);
						} else if (y % 2 == 1) {
							stamp.MaybeSetAt(x, y, Tile.Floor, "style:bookcase");
						} else {
							stamp.MaybeSetAt(x, y, Tile.Floor, "style:bookcase_end");
						}
					}
				}
			} else {
				for (int y = 0; y < stamp.H; y++) {
					for (int x = 0; x < stamp.W; x++) {
						if (x == 0 || y == 0 || x == stamp.W - 1 || y == stamp.H - 1) {
							stamp.MaybeSetAt(x, y, Tile.Wall);
						} else if (x % 2 == 0 && Mathf.Abs((stamp.H-1) / 2F - y) >= 1) {
							stamp.MaybeSetAt(x, y, Tile.Wall);
						} else if (x % 2 == 1) {
							stamp.MaybeSetAt(x, y, Tile.Floor, "style:bookcase");
						} else {
							stamp.MaybeSetAt(x, y, Tile.Floor, "style:bookcase_end");
						}
					}
				}
			}
		}
	}

	public class RoomSpecRounded : IRoomSpec {

		public RoomSpecRounded(int min, int max) {
			MinX = min;
			MinY = min;
			MaxX = max;
			MaxY = max;
		}

		public int MinX { get; set; }
		public int MinY { get; set; }
		public int MaxX { get; set; }
		public int MaxY { get; set; }

		public void StampRoom(Stamp stamp, IRandom rand) {
			int w = stamp.W - 2;
			int h = stamp.H - 2;
			float radW = ((w - 2) / 2F);
			float radH = ((h - 2) / 2F);
			float radW2 = radW * radW;
			float radH2 = radH * radH;
			for (int y = 1; y < h-1; y++) {
				for (int x = 1; x < w-1; x++) {
					float dx = (w / 2F) - x;
					float dy = (h / 2F) - y;
					if (dx * dx / radW2 + dy * dy / radH2 <= 1) {
						stamp.MaybeSetAt(x, y, Tile.Floor);
					}
				}
			}
			RoomSpecHelpers.Outline(stamp);
		}
	}

	public class RoomSpecRoundedCorners : IRoomSpec {

		public RoomSpecRoundedCorners(int min, int max) {
			MinX = min;
			MinY = min;
			MaxX = max;
			MaxY = max;
		}

		public int MinX { get; set; }
		public int MinY { get; set; }
		public int MaxX { get; set; }
		public int MaxY { get; set; }

		public void StampRoom(Stamp stamp, IRandom rand) {
			for (int y = 0; y < stamp.H; y++) {
				for (int x = 0; x < stamp.W; x++) {
					if (y == 0 || x == 0 || y == stamp.H - 1 || x == stamp.W - 1 || ((x == 1 || x == stamp.W - 2) && (y == 1 || y == stamp.H - 2))) {
						stamp.MaybeSetAt(x, y, Tile.Wall);
					} else {
						stamp.MaybeSetAt(x, y, Tile.Floor);
					}
				}
			}
		}
	}

	public class RoomSpecBlob : IRoomSpec {

		public RoomSpecBlob(int min, int max) {
			MinX = min;
			MinY = min;
			MaxX = max;
			MaxY = max;
		}

		public int MinX { get; set; }
		public int MinY { get; set; }
		public int MaxX { get; set; }
		public int MaxY { get; set; }

		public void StampRoom(Stamp stamp, IRandom rand) {
			float initialTileOdds = .5F;
			int iterations = 5;
			int requiredNeighbors = 5;
			Tile tileToSet = Tile.Floor;

			int w = stamp.W;
			int h = stamp.H;
			// TODO: Extract cellular automata code
			for (int iy = 1; iy < h - 1; iy++) {
				for (int ix = 1; ix < w - 1; ix++) {
					if (stamp.At(ix, iy) == Tile.Unset && rand.WithPercentChance(initialTileOdds)) {
						stamp.MaybeSetAt(ix, iy, tileToSet);
					}
				}
			}

			for (int i = 0; i < iterations; i++) {

				// List of points to turn into rooms
				List<Vector2Int> deadPoints = new List<Vector2Int>();
				// List of points to turn into walls
				List<Vector2Int> livePoints = new List<Vector2Int>();
				for (int iy = 1; iy < h - 1; iy++) {
					for (int ix = 1; ix < w - 1; ix++) {
						Vector2Int pt = new Vector2Int(ix, iy);
						Tile type = stamp.At(ix, iy);
						int wallNeighbors = pt.AdjacenciesWithDiag().Where(pt2 => stamp.At(pt2.x, pt2.y) == Tile.Unset).Count();
						if (wallNeighbors >= requiredNeighbors) livePoints.Add(pt);
						else deadPoints.Add(pt);
					}
				}
				deadPoints.ForEach(pt => stamp.MaybeSetAt(pt.x, pt.y, tileToSet));
				livePoints.ForEach(pt => stamp.MaybeSetAt(pt.x, pt.y, Tile.Unset));
			}

			RoomSpecHelpers.Outline(stamp);
		}
	}

	public static class RoomSpecHelpers {
		public static void Outline(Stamp stamp) {
			int h = stamp.H;
			int w = stamp.W;
			for (int y = 0; y < h; y++) {
				for (int x = 0; x < w; x++) {
					if (stamp.At(x, y) != Tile.Unset) continue;
					if (stamp.At(x - 1, y, Tile.Unset) == Tile.Floor
						|| stamp.At(x, y - 1, Tile.Unset) == Tile.Floor
						|| stamp.At(x, y + 1, Tile.Unset) == Tile.Floor
						|| stamp.At(x + 1, y, Tile.Unset) == Tile.Floor) {
						stamp.MaybeSetAt(x, y, Tile.Wall);
					}
				}
			}
		}

		public static Tile TileAt(Tile[,] tiles, int x, int y, Tile defaultValue) {
			if (x < 0 || y < 0 || x >= tiles.GetLength(1) || y >= tiles.GetLength(0)) return defaultValue;
			return tiles[y, x];
		}
	}
}