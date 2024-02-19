using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
	public interface IAlterer {
		/// <summary>
		/// Modifies the dungeon in some way. Can add rooms,
		/// add entities, etc. Most alterers will have some
		/// concept of an order they should be placed in, but
		/// it's not enforced.
		/// </summary>
		/// <param name="dungeon">The dungeon to modify</param>
		/// <param name="rand">Random number source</param>
		/// <returns>Return false if the dungeon generation should be aborted.</returns>
		bool Modify(TheDungeon dungeon, IRandom rand);
	}
}