using System.Collections.Generic;
using UnityEngine;
using Pomerandomian;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator {
	public class SourceRooms : ISource {
		private readonly float triesPerSquare;
		private readonly IRoomSpec[] specs;
		private readonly int[] odds;

		public SourceRooms(float triesPerSquare, IRoomSpec roomSpec) : this(triesPerSquare, new IRoomSpec[] { roomSpec }, new int[] { 1 }) {
		}

		public SourceRooms(float triesPerSquare, IRoomSpec[] roomSpecs, int[] odds) : base(Tile.Floor) {
			this.triesPerSquare = triesPerSquare;
			this.specs = roomSpecs;
			this.odds = odds;
		}

		public Stamp CreateRoom(IRoomSpec spec, int x, int y, int w, int h, IRandom rand) {
			Stamp stamp = new Stamp(x, y, w, h);
			spec.StampRoom(stamp, rand);
			return stamp;
		}

		public override void Generate(Stamp stamp, IRandom rand) {
			int roomTries = (int)(triesPerSquare * stamp.W * stamp.H);

			for (int i = 0; i < roomTries; i++) {

				IRoomSpec spec = rand.FromWithOdds(specs, odds);

				int innerW = rand.Next(spec.MinX, spec.MaxX + 1);
				int innerH = rand.Next(spec.MinY, spec.MaxY + 1);

				// Account for walls
				int wr = innerW + 2;
				int hr = innerH + 2;

				int maxX = stamp.W - 1 - wr;
				int maxY = stamp.H - 1 - hr;
				if (maxX < 1 || maxY < 1) {
					continue;
				}
				int x = rand.Next(1, maxX + 1);
				int y = rand.Next(1, maxY + 1);

				// Since we're checking this first and not the tiles,
				// it will place rooms less densely, but we won't generate
				// rooms as much (probably).
				if (!canPlaceRoom(stamp, x, y, wr, hr)) {
					continue;
				}

				Stamp room = CreateRoom(spec, x, y, wr, hr, rand);
				stamp.GetStamped(room);
				Debug.Log("Stamping room " + spec + ":\n" + TheDungeon.Visualize(room.Tiles));
			}
		}

		private bool canPlaceRoom(Stamp stamp, int x, int y, int w, int h) {
			for (int iy = y; iy < y + h; iy++) {
				for (int ix = x; ix < x + w; ix++) {
					if (stamp.Tiles[iy, ix] == Tile.Floor) return false;
					if (!stamp.CanSetAt(ix, iy)) return false;
				}
			}
			return true;
		}
	}
}