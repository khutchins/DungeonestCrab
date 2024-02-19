using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
	/// <summary>
	/// Consumes the failure of the child alterer.
	/// Care should be given that it isn't consuming
	/// an alterer that leaves inbetween state.
	/// </summary>
	public class Try : IAlterer {
		readonly IAlterer _altererToDo;

		public Try(IAlterer altererToDo) {
			this._altererToDo = altererToDo;
		}

		public bool Modify(TheDungeon generator, IRandom rand) {
			_altererToDo.Modify(generator, rand);
			return true;
		}
	}
}