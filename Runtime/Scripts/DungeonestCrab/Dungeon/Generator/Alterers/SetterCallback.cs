using UnityEngine;
using Pomerandomian;
using System;

namespace DungeonestCrab.Dungeon.Generator {

	/// <summary>
	/// Allows setting dungeon values explicitly through a callback. Useful for making one-off changes.
	/// </summary>
	public class SetterCallback : IAlterer {
		private readonly Func<TheDungeon, IRandom, bool> _callback;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="callback">Arbitrary callback for modifying the dungeon explicitly. Returns whether or not it was successful.</param>
		public SetterCallback(Func<TheDungeon, IRandom, bool> callback) {
			_callback = callback;
		}

		public bool Modify(TheDungeon generator, IRandom rand) {
			return _callback(generator, rand);
		}
	}
}