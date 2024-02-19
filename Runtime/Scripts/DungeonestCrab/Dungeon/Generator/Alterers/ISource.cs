using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
	public abstract class ISource {
		protected readonly Tile _tileToSet;

		public ISource(Tile tileToSet) {
			_tileToSet = tileToSet;
		}

		public abstract void Generate(Stamp stamp, IRandom rand);
	}
}
