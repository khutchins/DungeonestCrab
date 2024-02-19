using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;
using System.Text;
using System;
using DungeonestCrab;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator {

	public class DungeonSpec {
		public readonly List<IAlterer> alterers;
		public readonly Trait trait;
		public readonly Vector2Int size;
		public readonly float fogDensity;
		public readonly Color fogColor;

		public DungeonSpec(Trait trait, Vector2Int size, IAlterer[] alterers, float fogDensity = 0.1F, Color? fogColor = null) {
			this.alterers = new List<IAlterer>(alterers);
			this.size = size;
			this.trait = trait;
			this.fogDensity = fogDensity;
			this.fogColor = fogColor ?? Color.black;
		}
	}

	public class TheGenerator {

		public readonly DungeonSpec _dungeonSpec;

		public TheGenerator(DungeonSpec ds) {
			_dungeonSpec = ds;
		}

		public TheDungeon Generate(IRandom random) {
			IAlterer[] dungeonAlterers = _dungeonSpec.alterers.ToArray();
			int attempts = 20;
			bool dungeonFailed = true;
			TheDungeon dungeon;
			for (int i = 0; i < attempts && dungeonFailed; i++) {
				var fullWatch = System.Diagnostics.Stopwatch.StartNew();
				dungeonFailed = false;

				dungeon = new TheDungeon(_dungeonSpec.size.x, _dungeonSpec.size.y, random);
				dungeon.Trait = _dungeonSpec.trait;
				dungeon.FogColor = _dungeonSpec.fogColor;
				dungeon.FogDensity = _dungeonSpec.fogDensity;

				foreach (IAlterer alterer in dungeonAlterers) {
					var watch = System.Diagnostics.Stopwatch.StartNew();
					if (!alterer.Modify(dungeon, dungeon.ConsistentRNG)) {
						Debug.LogWarning("Failed generation due to " + alterer);
						dungeonFailed = true;
						break;
					}
					watch.Stop();
					Debug.LogFormat("{0} finished in {1}s", alterer, watch.ElapsedMilliseconds / 1000F);
					Debug.Log(dungeon.Visualize());
				}

				if (!dungeonFailed) {
					dungeon.UpdateDungeonComputations();
				}

				fullWatch.Stop();
				Debug.LogFormat("Generation finished in {0}s", fullWatch.ElapsedMilliseconds / 1000F);

				if (!dungeonFailed) {
					return dungeon;
				}
			}

			Debug.LogErrorFormat("Dungeon could not successfully complete after {0} attempts!", attempts);
			return null;
		}
    }
}