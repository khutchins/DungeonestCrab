using DungeonestCrab.Dungeon;
using KH.Graph;
using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
	// This class is based on https://github.com/munificent/hauberk's Forest architecture.
	// See LICENSE-Hauberk for the details.
	public class SourceVoronoi : ISource {
		private readonly int _cells;
		private readonly int _regionSize;
		private readonly int _iterations;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tile">Tile to set.</param>
		/// <param name="cells">The number of circles to add.</param>
		/// <param name="regionSize">The size of each drawn region.</param>
		/// <param name="iterations">The amount of iterations to run. More iterations means more uniformly placed cells.</param>
		public SourceVoronoi(Tile tile, int cells, int regionSize, int iterations) : base(tile) {
			_cells = cells;
			_regionSize = regionSize;
			_iterations = iterations;
		}

		public override void Generate(Stamp stamp, IRandom rand) {
			if (_cells < 1) return;

			List<Vector2Int> points = new List<Vector2Int>();

			for (int i = 0; i < _cells; i++) {
				points.Add(new Vector2Int(rand.Next(0, stamp.W), rand.Next(0, stamp.H)));
			}

			for (int i = 0; i < _iterations; i++) {
				var regions = new List<List<Vector2Int>>();
				for (int r = 0; r < _cells; r++) {
					regions.Add(new List<Vector2Int>());
				}

				// Compute the region each point belongs to.
				// (They belong to the region defined by the
				// closest cell center).
				foreach (Vector2Int coord in stamp.All()) {
					int nearestRegion = 0;

					float minDist2 = int.MaxValue;

					for (int c = 0; c < points.Count; c++) {
						float cDist2 = (points[c] - coord).sqrMagnitude;
						if (cDist2 < minDist2) {
							minDist2 = cDist2;
							nearestRegion = c;
						}
					}

					regions[nearestRegion].Add(coord);
				}

				// Move each cell center to the center of the
				// region defined by their cells.
				for (int c = 0; c < _cells; c++) {
					int aggx = 0;
					int aggy = 0;
					foreach (Vector2Int coord in regions[c]) {
						aggx += coord.x;
						aggy += coord.y;
					}

					points[c] = new Vector2Int(aggx / regions[c].Count, aggy / regions[c].Count);
				}
			}

			foreach(Vector2Int c in points) {
				stamp.StampCircle(_tileToSet, c, _regionSize);
			}

			UndirectedGraph<Vector2Int> graph = Vector2IntCoordExtensions.CliqueGraph(points);

			foreach(var edge in graph.MinimumSpanningTree()) {
				stamp.StampLine(_tileToSet, edge.N1.Element, edge.N2.Element);
			}
		}
	}
}