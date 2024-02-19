using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pomerandomian;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator {
	/// <summary>
	/// Fills the region that it's in with a circle centered at the middle.
	/// </summary>
	public class SourceCircle : ISource {

		public SourceCircle(Tile tileToSet) : base(tileToSet) {
		}

		public override void Generate(Stamp stamp, IRandom rand) {
			int min = Mathf.Min(stamp.W, stamp.H);
			float rad = min / 2f;

			StampCircle(stamp, _tileToSet, stamp.W / 2f, stamp.H / 2f, rad);
		}

		public static void StampCircle(Stamp stamp, Tile tile, float cx, float cy, float rad) {
			float rad2 = rad * rad;

			for (int ix = Mathf.FloorToInt(cx - rad); ix < cx + rad; ix++) {
				for (int iy = Mathf.FloorToInt(cy - rad); iy < cy + rad; iy++) {
					float mx = ix + 0.5f;
					float my = iy + 0.5f;
					if ((cx - mx) * (cx - mx) + (cy - my) * (cy - my) < rad2) {
						stamp.MaybeSetAt(ix, iy, tile);
					}
				}
			}
		}
	}
}