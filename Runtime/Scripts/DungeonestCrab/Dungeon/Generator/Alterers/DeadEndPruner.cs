using System.Collections.Generic;
using System.Linq;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
	public class DeadEndPruner : IAlterer {
		public bool Modify(TheDungeon generator, IRandom rand) {
			while (true) {
				bool hadItem = false;
				foreach (TileSpec spec in DeadEndPruner.GetDeadEnds(generator)) {
					spec.Tile = Tile.Wall;
					hadItem = true;
				}
				if (!hadItem) return true;
			}
		}

		public static IEnumerable<TileSpec> GetDeadEnds(TheDungeon generator) {
			foreach (TileSpec tile in generator.AllTiles()) {
				if (tile.Tile != Tile.Floor || tile.Immutable || tile.Style != 0) continue;
				
				if (generator.CardinalAdjacentWalkableList(tile.Coords).Count(x => x) < 2) {
					yield return tile;
				}
			}
		}
	}
}