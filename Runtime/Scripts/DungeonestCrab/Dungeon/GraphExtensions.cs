using KH.Graph;
using UnityEngine;

namespace DungeonestCrab.Dungeon {
    public class GraphExtensions : MonoBehaviour {
		public static DirectedGraph<Vector2Int> FromCostMap(float[,] costMap) {
			int width = costMap.GetLength(0);
			int height = costMap.GetLength(1);

			DirectedGraph<Vector2Int> graph = new DirectedGraph<Vector2Int>();

			DirectedGraph<Vector2Int>.Node[,] node2D = new DirectedGraph<Vector2Int>.Node[width, height];

			// Add nodes
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					node2D[x, y] = graph.AddNode(new Vector2Int(x, y));
				}
			}

			// Add edges
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					var thisNode = node2D[x, y];
					var cost = costMap[x, y];

					// Costs less than zero cannot be traversed.
					if (cost < 0) {
						continue;
					}
					// Add all incoming nodes.
					if (x > 0) graph.AddEdge(node2D[x - 1, y], thisNode, costMap[x, y]);
					if (x < width - 1) graph.AddEdge(node2D[x + 1, y], thisNode, costMap[x, y]);
					if (y > 0) graph.AddEdge(node2D[x, y - 1], thisNode, costMap[x, y]);
					if (y < height - 1) graph.AddEdge(node2D[x, y + 1], thisNode, costMap[x, y]);
				}
			}

			return graph;
		}
	}
}