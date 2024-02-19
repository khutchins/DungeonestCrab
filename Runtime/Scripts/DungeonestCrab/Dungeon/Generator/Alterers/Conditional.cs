using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
	/// <summary>
	/// Conditionally performs alterer.
	/// </summary>
	public class Conditional : IAlterer {
		readonly IAlterer altererToDo;
		readonly Func<TheDungeon, IRandom, bool> shouldPerform;

		public Conditional(IAlterer altererToDo, Func<TheDungeon, IRandom, bool> shouldPerform) {
			this.altererToDo = altererToDo;
			this.shouldPerform = shouldPerform;
		}

		public bool Modify(TheDungeon generator, IRandom rand) {
			if (this.shouldPerform(generator, rand)) {
				return this.altererToDo.Modify(generator, rand);
			}
			return true;
		}
	}
}