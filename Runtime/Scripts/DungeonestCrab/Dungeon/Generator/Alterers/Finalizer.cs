using System.Linq;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
	public class Finalizer : IAlterer {
		private readonly TerrainSO _defaultTerrain;

		public Finalizer(TerrainSO defaultTerrain) {
			this._defaultTerrain = defaultTerrain;
		}

		public bool Modify(TheDungeon generator, IRandom rand) {
			foreach (TileSpec tile in generator.AllTiles()) {
				tile.SetTerrainIfNull(_defaultTerrain);
			}
			return true;
		}
	}
}