using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator {
	/// <summary>
	/// Generates straight lines (with a chance of curving) from one end to the other.
	/// 
	/// Example use: Minecard tracks
	/// </summary>
	public class SourceStripe : ISource {
		readonly float _stripeDensityPerSide;
		readonly float _curveOdds;
		readonly float _deadEndOddPerSquare;
		float _maxTilesPerStripe;

        /// <summary>
        /// Constructor for the stripe source.
        /// </summary>
        /// <param name="tileToSet">Tile to generate</param>
        /// <param name="stripeDensityPerSide">Percent of tiles per side that should be stripe sources</param>
        /// <param name="curveOdds">Odds that the track will turn at any given square</param>
        /// <param name="deadEndOddPerSquare">Odds that the track will terminate at any given square</param>
        public SourceStripe(Tile tileToSet, float stripeDensityPerSide, float curveOdds, float deadEndOddPerSquare) : base(tileToSet) {
			_stripeDensityPerSide = stripeDensityPerSide;
			_curveOdds = curveOdds;
			_deadEndOddPerSquare = deadEndOddPerSquare;
		}

		public override void Generate(Stamp stamp, IRandom rand) {
			int w = stamp.W;
			int h = stamp.H;
			this._maxTilesPerStripe = w * h;
			int numX = (int)(w * this._stripeDensityPerSide);
			int numY = (int)(h * this._stripeDensityPerSide);

			IEnumerable<int> topIdxs = Enumerable.Range(0, w - 1).Shuffle(rand).Take(numX);
			foreach (int i in topIdxs) {
				CarveStripe(stamp, new DirectedPoint(i, 0, Dir.Down), rand);
			}
			IEnumerable<int> botIdxs = Enumerable.Range(0, w - 1).Shuffle(rand).Take(numX);
			foreach (int i in botIdxs) {
				CarveStripe(stamp, new DirectedPoint(i, h - 1, Dir.Up), rand);
			}
			IEnumerable<int> leftIdxs = Enumerable.Range(0, h - 1).Shuffle(rand).Take(numY);
			foreach (int i in leftIdxs) {
				CarveStripe(stamp, new DirectedPoint(0, i, Dir.Right), rand);
			}
			IEnumerable<int> rightIdxs = Enumerable.Range(0, h - 1).Shuffle(rand).Take(numY);
			foreach (int i in rightIdxs) {
				CarveStripe(stamp, new DirectedPoint(w - 1, i, Dir.Left), rand);
			}
		}

		private void CarveStripe(Stamp stamp, DirectedPoint pt, IRandom rand, int currentNum = 0) {
			DirectedPoint currentPt = pt;
			int paint = pt.IsVertical() ? (int)PaintStyle.FloorNS : (int)PaintStyle.FloorEW;

			// Should terminate, probably. This will make sure it does.
			for (int i = currentNum; i < this._maxTilesPerStripe; i++) {
				if (!stamp.CanSetAt(currentPt)) {
					return;
				}

				stamp.MaybeSetDirectional(currentPt, _tileToSet, pt.dir);

				if (rand.WithPercentChance(_deadEndOddPerSquare)) {
					return;
				}
				if (rand.WithPercentChance(_curveOdds)) {
					Dir newDir = AdjDir(pt.dir, rand);
					stamp.MaybeSetDirectional(currentPt, _tileToSet, newDir);
					CarveStripe(stamp, currentPt.InDir(newDir), rand, i);
					return;
				}

				currentPt = currentPt.InCurrentDir();
			}
		}

		private Dir AdjDir(Dir curr, IRandom rand) {
			bool toLeft = rand.NextBool();
			switch (curr) {
				case Dir.Left:
					return toLeft ? Dir.Down : Dir.Up;
				case Dir.Right:
					return toLeft ? Dir.Up : Dir.Down;
				case Dir.Up:
					return toLeft ? Dir.Left : Dir.Right;
				case Dir.Down:
					return toLeft ? Dir.Right : Dir.Left;
			}

			// Should not happen
			return Dir.Down;
		}
	}
}