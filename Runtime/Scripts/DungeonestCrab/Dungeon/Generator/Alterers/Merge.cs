using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
	// TODO: Why does this class exist?
    public class Merge : IAlterer {

		private readonly List<IAlterer> _alterers = new List<IAlterer>();

		public Merge(IEnumerable<IAlterer> alterers) {
			_alterers.AddRange(alterers);
		}

		public bool Modify(TheDungeon generator, IRandom rand) {
			foreach (IAlterer alterer in _alterers) {
				if (!alterer.Modify(generator, rand)) {
					return false;
				}
			}
			return true;
		}
    }
}