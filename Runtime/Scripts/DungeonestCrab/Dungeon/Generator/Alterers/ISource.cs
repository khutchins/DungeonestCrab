using DungeonestCrab.Dungeon;
using Pomerandomian;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
	[System.Serializable]
	public abstract class ISource {
		[SerializeField] protected Tile _tileToSet;

		public ISource(Tile tileToSet) {
			_tileToSet = tileToSet;
		}

		public abstract void Generate(Stamp stamp, IRandom rand);
	}
}
