using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
	/// <summary>
	/// Connects all distinct regions. This will only combine regions that are touching,
	/// so make sure that all empty space is filled if you want a connected dungeon.
	/// </summary>
	public class AdjacentCombiner : IAlterer {

		private readonly double _chanceOfExtraJunction;
		private readonly TerrainSO _terrain;

		public AdjacentCombiner(TerrainSO terrain, double chanceOfExtraJunction) {
			this._chanceOfExtraJunction = chanceOfExtraJunction;
			this._terrain = terrain;
		}

		public bool Modify(TheDungeon generator, IRandom rand) {
			int maxRegion = -1;
			int[,] tileRegionMap = generator.ComputeRegions(out maxRegion);

			Dictionary<Vector2Int, List<int>> connRegions = new Dictionary<Vector2Int, List<int>>();
			foreach (TileSpec spec in generator.AllTiles()) {
				if (spec.Tile != Tile.Wall) continue;
				Vector2Int pt = spec.Coords;

				List<int> connRegion = pt.Adjacencies1Away().Where(
					pt2 => generator.Contains(pt2.x, pt2.y) 
					&& tileRegionMap[pt2.y, pt2.x] != -1 
					&& tileRegionMap[pt2.y, pt2.x] != tileRegionMap[pt.y, pt.x]
				).Select(pt2 => tileRegionMap[pt2.y, pt2.x]).Distinct().ToList();

				if (connRegion.Count > 1) {
					connRegions[pt] = connRegion;
				}
			}

			List<Vector2Int> connectors = new List<Vector2Int>(connRegions.Keys);
			List<int> openRegions = new List<int>();

			Dictionary<int, int> regionMap = new Dictionary<int, int>();
			for (int i = 0; i <= maxRegion; i++) {
				regionMap[i] = i;
				openRegions.Add(i);
			}

			while (openRegions.Count > 1) {
				if (connectors.Count == 0) {
					Debug.LogError("Out of connectors. Not all regions are connected!");
					return true;
				}
				Vector2Int connector = rand.From(connectors);
				generator.AddJunction(connector, _terrain);

				// Get all regions connected by connector
				List<int> regions = connRegions[connector].Select(r => regionMap[r]).ToList();
				int startRegion = regions[0];
				List<int> endRegions = regions.Skip(1).ToList();

				// Update index of regions to prevent double join
				for (int i = 0; i <= maxRegion; i++) {
					if (endRegions.Contains(regionMap[i])) {
						regionMap[i] = startRegion;
					}
				}

				for (int i = 0; i < endRegions.Count; i++) {
					openRegions.Remove(endRegions[i]);
				}

				connectors.RemoveAll(c => {
					// Too close to previous junction
					if (Mathf.Abs(c.x - connector.x) + Mathf.Abs(c.y - connector.y) < 2) return true;

					// Enough connecting regions
					if (connRegions[c].Select(x => regionMap[x]).Distinct().Count() > 1) return false;

					// We want to keep some extra entrances.
					if (rand.NextDouble() < _chanceOfExtraJunction) {
						generator.AddJunction(c, _terrain);
					}

					return true;
				});
			}

			return true;
		}
	}
}