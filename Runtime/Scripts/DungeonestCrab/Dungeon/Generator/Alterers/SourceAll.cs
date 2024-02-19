using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator {
	public class SourceAll : ISource {
		public SourceAll(Tile tts) : base(tts) { }

		public override void Generate(Stamp stamp, IRandom rand) {
			for (int iy = 0; iy < stamp.H; iy++) {
				for (int ix = 0; ix < stamp.W; ix++) {
					stamp.MaybeSetAt(ix, iy, _tileToSet);
				}
			}
		}
    }
}