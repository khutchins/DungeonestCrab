using System.Collections.Generic;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
	public class SourceRooms : ISource {
		private readonly float triesPerSquare;
		private readonly RoomStrategy[] strategies;
		private readonly int[] odds;

        public SourceRooms(float triesPerSquare, List<RoomStrategy> strats) : base(Tile.Floor) {
            this.triesPerSquare = triesPerSquare;
            this.strategies = strats != null ? strats.ToArray() : new RoomStrategy[] {};

            odds = new int[strategies.Length];
            for (int i = 0; i < strategies.Length; i++) {
                odds[i] = strategies[i].Weight;
            }
        }

        public override void Generate(Stamp stamp, IRandom rand) {
            if (strategies.Length == 0) return;

            int roomTries = (int)(triesPerSquare * stamp.W * stamp.H);

            for (int i = 0; i < roomTries; i++) {
                RoomStrategy strat = rand.FromWithOdds(strategies, odds);

				int innerW = rand.Next(strat.MinX, strat.MaxX + 1);
				int innerH = rand.Next(strat.MinY, strat.MaxY + 1);
                // +2 for Walls
                int wr = innerW + 2;
                int hr = innerH + 2;

                int maxX = stamp.W - 1 - wr;
                int maxY = stamp.H - 1 - hr;
                if (maxX < 1 || maxY < 1) continue;

                int x = rand.Next(1, maxX + 1);
                int y = rand.Next(1, maxY + 1);

                if (!CanPlaceRoom(stamp, x, y, wr, hr)) continue;

                Stamp room = CreateRoom(strat, x, y, wr, hr, rand);
                stamp.GetStamped(room);
            }
        }

        public Stamp CreateRoom(RoomStrategy strategy, int x, int y, int w, int h, IRandom rand) {
            Stamp stamp = new Stamp(x, y, w, h);
            strategy.StampRoom(stamp, rand);
            return stamp;
        }

        private bool CanPlaceRoom(Stamp stamp, int x, int y, int w, int h) {
            for (int iy = y; iy < y + h; iy++) {
                for (int ix = x; ix < x + w; ix++) {
                    if (stamp.At(ix, iy) == Tile.Floor) return false;
                    if (!stamp.CanSetAt(ix, iy)) return false;
                }
            }
            return true;
        }
    }
}