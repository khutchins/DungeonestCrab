using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;
using KH.Noise;
using KH.Graph;

namespace DungeonestCrab.Dungeon.Generator {
	/// <summary>
	/// Will connect all regions in a map. Does not require the
	/// dungeon to be filled.
	/// </summary>
	public abstract class AStarAbstractCombiner : IAlterer {
		private readonly float chanceOfExtraPath;
		private readonly TerrainSO terrain;

		protected readonly float floorCost = 1;
		protected readonly float additionalCarveCost = 0;
		protected readonly INoiseSource carveNoise = null;
		protected readonly float carveNoiseMultiplier = 1;

		/// <summary>
		/// Combines path given 
		/// </summary>
		/// <param name="terrain">Type of terrain to create on path (only creates if unset).</param>
		/// <param name="chanceOfExtraPath">The chance of connecting a region to another multiple times. If floor cost is low enough, this will not occur.</param>
		/// <param name="floorCost">The cost of traversing the floor. As this deviates down from 1, it will reuse existing paths more. As it deviates up from 1, it will choose a worse path more often.</param>
		/// <param name="additionalCarveCost">The cost to carve a new tile. Further discourages pathing along new routes.</param>
		/// <param name="carveNoise">Whether noise should be applied to carve costs to break up paths</param>
		/// <param name="carveNoiseMultiplier">Multiplier on carve noise (default range is 0-1)</param>
		public AStarAbstractCombiner(TerrainSO terrain, float chanceOfExtraPath = 0F, float floorCost = 1, float additionalCarveCost = 0, INoiseSource carveNoise = null, float carveNoiseMultiplier = 1) {
			// Cap this at 0.9 as we want it to terminate in a somewhat
			// reasonable amount of time.
			this.terrain = terrain;
			this.chanceOfExtraPath = Mathf.Min(chanceOfExtraPath, 0.9F);
			this.floorCost = floorCost;
			this.additionalCarveCost = additionalCarveCost;
			this.carveNoise = carveNoise;
			this.carveNoiseMultiplier = carveNoiseMultiplier;
		}

		private DirectedGraph<Vector2Int>.Node NodeFromCoords(DirectedGraph<Vector2Int> graph, Vector2Int c) {
			return graph.Nodes.Where(x => x.Element.x == c.x && x.Element.y == c.y).FirstOrDefault();
		}

		public bool Modify(TheDungeon generator, IRandom rand) {
			List<Vector2Int> points = PointsToConnect(generator, rand);
			carveNoise?.SetSeed(rand.Next(1000000000));

			DirectedGraph<Vector2Int> graph = GraphExtensions.FromCostMap(CostMap(generator, rand));

			while (points.Count > 1) {
				points = points.Shuffle(rand).ToList();

				Vector2Int from = points[0];
				Vector2Int to = points[1];

				Debug.Log(string.Format("Combining {0} with {1}", from, to));

				var path = graph.FindPath(NodeFromCoords(graph, from), NodeFromCoords(graph, to), (Vector2Int c1, Vector2Int c2) => {
					int dx = c2.x - c1.x;
					int dy = c2.y - c1.y;
					return Mathf.Abs(dx) + Mathf.Abs(dy);
				});

				if (path != null) {
					CarvePath(generator, path);
				} else {
					Debug.LogWarning(string.Format("Unable to path from {0} to {1}!", from, to));
				}

				if (!rand.WithPercentChance(chanceOfExtraPath)) {
					points.RemoveAt(0);
				}
			}
			return true;
		}

		protected List<Vector2Int> PointsToConnect(TheDungeon gen, IRandom rand) {
			int maxRegions;
			int[,] regions = gen.ComputeRegions(out maxRegions);

			// This is probably very inefficient
			Dictionary<int, List<Vector2Int>> pointMap = new Dictionary<int, List<Vector2Int>>();
			for (int i = 0; i <= maxRegions; i++) {
				pointMap[i] = new List<Vector2Int>();
			}

			foreach (TileSpec spec in gen.AllTiles()) {
				if (regions[spec.Coords.y, spec.Coords.x] < 0) continue;
				pointMap[regions[spec.Coords.y, spec.Coords.x]].Add(spec.Coords);
			}

			List<Vector2Int> pointsToConnect = new List<Vector2Int>();
			for (int i = 0; i <= maxRegions; i++) {
				pointsToConnect.Add(rand.From(pointMap[i]));
			}
			return pointsToConnect;
		}

		protected void CarvePath(TheDungeon gen, List<DirectedGraph<Vector2Int>.Node> path) {
			foreach (var pt in path) {
				foreach (DirectedGraph<Vector2Int>.Edge edge in pt.IncomingEdges) {
					edge.Cost = floorCost;
				}
				// TODO: Handle junctions (maybe at different terrain types?)
				TileSpec tile = gen.GetTileSpec(pt.Element);
				tile.Tile = Tile.Floor;
				if (tile.Terrain == null) tile.Terrain = terrain;
			}
		}

		protected float[,] CostMap(TheDungeon gen, IRandom rand) {
			float[,] costMap = new float[gen.Size.x, gen.Size.y];

			for (int y = 0; y < gen.Size.y; y++) {
				for (int x = 0; x < gen.Size.x; x++) {
					costMap[x, y] = CostForTile(x, y, gen);
				}
			}
			return costMap;
		}

		/// <summary>
		/// Retrieve cost for each tile. It will only be called once per tile, so having somewhat random results will not give an inconsistent map.
		/// </summary>
		public abstract float CostForTile(int x, int y, TheDungeon gen);
	}
}