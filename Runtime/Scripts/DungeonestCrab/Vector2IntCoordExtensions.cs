using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KH.Graph;

namespace DungeonestCrab {
	public static class Vector2IntCoordExtensions {
		/// <summary>
		/// Returns whether or not pt is within the bounds of this Vector2Int,
		/// defined by reaching from [0, v.x) in the x dimension and [0, v.y)
		/// in the y dimension.
		/// </summary>
		public static bool InBounds(this Vector2Int v, Vector2Int pt) {
			return pt.x >= 0 && pt.y >= 0 && pt.x < v.x && pt.y < v.y;
		}

		/// <summary>
		/// Returns whether or not the coords are within the bounds of this Vector2Int,
		/// defined by reaching from [0, v.x) in the x dimension and [0, v.y)
		/// in the y dimension.
		/// </summary>
		public static bool InBounds(this Vector2Int v, int x, int y) {
			return x >= 0 && y >= 0 && x < v.x && y < v.y;
		}

		public static IEnumerable<Vector2Int> AdjacenciesWithDiag(this Vector2Int v) {
			yield return new Vector2Int(v.x - 1, v.y - 1);
			yield return new Vector2Int(v.x - 1, v.y);
			yield return new Vector2Int(v.x - 1, v.y + 1);
			yield return new Vector2Int(v.x, v.y - 1);
			yield return new Vector2Int(v.x, v.y + 1);
			yield return new Vector2Int(v.x + 1, v.y - 1);
			yield return new Vector2Int(v.x + 1, v.y);
			yield return new Vector2Int(v.x + 1, v.y + 1);
		}

		public static IEnumerable<Vector2Int> Adjacencies1Away(this Vector2Int v) {
			yield return new Vector2Int(v.x - 1, v.y);
			yield return new Vector2Int(v.x + 1, v.y);
			yield return new Vector2Int(v.x, v.y - 1);
			yield return new Vector2Int(v.x, v.y + 1);
		}

		public static IEnumerable<Vector2Int> Adjacencies2Away(this Vector2Int v) {
			yield return new Vector2Int(v.x - 2, v.y);
			yield return new Vector2Int(v.x + 2, v.y);
			yield return new Vector2Int(v.x, v.y - 2);
			yield return new Vector2Int(v.x, v.y + 2);
		}

		public static IEnumerable<Vector2Int> Range(int x, int y, int width, int height) {
			for (int ix = x; ix < x + width; ix++) {
				for (int iy = y; iy < y + height; iy++) {
					yield return new Vector2Int(ix, iy);
				}
			}
		}

		public static IEnumerable<Vector2Int> Bounds(int x, int y, int toXExclusive, int toYExclusive) {
			return Range(x, y, toXExclusive - x, toYExclusive - y);
		}

		public static UndirectedGraph<Vector2Int> CliqueGraph(List<Vector2Int> coords) {
			UndirectedGraph<Vector2Int> graph = new UndirectedGraph<Vector2Int>();
			List<UndirectedGraph<Vector2Int>.Node> nodes = new List<UndirectedGraph<Vector2Int>.Node>();

			for (int i = 0; i < coords.Count; i++) {
				Vector2Int c1 = coords[i];
				nodes.Add(graph.AddNode(c1));
			}

			for (int i = 0; i < nodes.Count; i++) {
				var n1 = nodes[i];

				for (int j = i + 1; j < nodes.Count; j++) {
					var n2 = nodes[j];
					graph.AddEdge(n1, n2, (n2.Element - n1.Element).sqrMagnitude);
				}
			}

			return graph;
		}

		public static IEnumerable<Vector2Int> Line(this Vector2Int v, Vector2Int to) {
			Vector2 dir = new Vector2(to.x - v.x, to.y - v.y);
			float dist = dir.magnitude;

			foreach (Vector2Int coord in v.Ray(dir, dist)) {
				yield return coord;
			}
		}

		/// <summarv.y>
		/// Generates a ray that begins at the current coords and goes tile by tile
		/// until dist is reached. If dist is not provided, it will continue indefinitely.
		/// </summarv.y>
		/// <param name="dir">The direction of the ray. Does not need to be normalized.</param>
		/// <param name="dist">The distance of the ray.</param>
		/// <returns>An enumerable that returns points on the line forever.</returns>
		public static IEnumerable<Vector2Int> Ray(this Vector2Int v, Vector2 dir, float dist = int.MaxValue) {
			dir.Normalize();

			float fx = v.x;
			float fy = v.y;

			for (int i = 0; i <= dist; i++) {
				yield return new Vector2Int(Mathf.FloorToInt(fx), Mathf.FloorToInt(fy));
				fx += dir.x;
				fy += dir.y;
			}
		}
	}
}