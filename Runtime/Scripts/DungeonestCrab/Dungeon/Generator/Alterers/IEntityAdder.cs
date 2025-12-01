using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator {
	public abstract class IEntityAdder : IAlterer {
		private static bool AcceptableToBlock(TheDungeon gen, Vector2Int pt) {
			bool[] adj = gen.AdjacentWalkableList(pt);

			// We start at the last true index, as that will allow us
			// to determine six true values in a row across the array
			// boundaries.
			int startIdx = Array.LastIndexOf(adj, true);
			// No true values.
			if (startIdx < 0) return false;

			bool lastValueRead = true;
			int partitions = 1;
			for (int i = 0; i < adj.Length; i++) {
				if (adj[(i + startIdx) % adj.Length] != lastValueRead) {
					lastValueRead = !lastValueRead;
					partitions++;
				}
			}
			return partitions <= 2;
		}

		protected static void PlaceAt(TheDungeon generator, IRandom rand, EntitySource source, List<Vector2Int> coords) {
			int entityIndex = 0;
			foreach(Vector2Int coord in coords) {
				EntitySource.Pair pair = source.GetPair(rand);
				generator.AddEntity(new Entity(coord, entityIndex++, pair.Entity, pair.Code));
			}
		}

		protected static bool PlaceMany(TheDungeon generator, IRandom rand, EntitySource source, IMatcher matcher, bool placeToNotBlockDungeon, int minRequired, int targetAmt) {
			if (targetAmt <= 0) return true;
			int actualMin = targetAmt < minRequired ? targetAmt : minRequired;

			List<Vector2Int> spawnablePoints = new List<Vector2Int>();

			foreach (TileSpec spec in generator.AllTiles()) {
				if (spec.Immutable) continue;
				if (!matcher.Matches(spec)) continue;
				if (placeToNotBlockDungeon && spec.EntityBlocksMovement()) {
					continue;
				}
				if (placeToNotBlockDungeon && !AcceptableToBlock(generator, spec.Coords)) {
					continue;
				}
				spawnablePoints.Add(spec.Coords);
			}

			spawnablePoints = spawnablePoints.Shuffle(rand).ToList();
			int i = 0;
			for (i = 0; i < targetAmt && spawnablePoints.Count > 0; i++) {
				Vector2Int pt = spawnablePoints[0];
				EntitySource.Pair pair = source.GetPair(rand);
                if (pair == null) {
                    Debug.LogWarning($"EntitySource {source} returned null.");
                    return false;
                }
                generator.AddEntity(new Entity(pt, i, pair.Entity, pair.Code));
				spawnablePoints.RemoveAll(pt2 => pt == pt2 || pt2.AdjacenciesWithDiag().Contains(pt));
			}
			Debug.Log(string.Format("Generated {0} entity of target {1}", i, targetAmt));
			return i >= minRequired;
		}

		public abstract bool Modify(TheDungeon generator, IRandom rand);
	}
}