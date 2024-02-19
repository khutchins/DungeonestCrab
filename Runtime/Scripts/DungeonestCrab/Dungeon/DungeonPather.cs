using KH.Graph;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon {
    public class DungeonPather {
        public delegate float CostForTile(TheDungeon generator, int x, int y);

        private TheDungeon _generator;
        private CostForTile _costCallback;
		private DirectedGraph<Vector2Int> _graph;

		public DungeonPather(TheDungeon generator, CostForTile costCallback) {
			_generator = generator;
			_costCallback = costCallback;
			_graph = GraphExtensions.FromCostMap(CostMap(_generator));
		}

		private DirectedGraph<Vector2Int>.Node NodeFromCoords(DirectedGraph<Vector2Int> graph, Vector2Int c) {
			return graph.Nodes.Where(x => x.Element.x == c.x && x.Element.y == c.y).FirstOrDefault();
		}

		public IEnumerable<Vector2Int> FindPath(Vector2Int from, Vector2Int to) {
			var path = _graph.FindPath(NodeFromCoords(_graph, from), NodeFromCoords(_graph, to), (Vector2Int c1, Vector2Int c2) => {
				int dx = c2.x - c1.x;
				int dy = c2.y - c1.y;
				return Mathf.Abs(dx) + Mathf.Abs(dy);
			});

			if (path == null) return null;

			return path.Select(x => x.Element);
		}
		
		public IEnumerable<Vector2Int> FindPathToFarthestPoint(Vector2Int from) {
			return _graph.FindPathToFarthestNode(NodeFromCoords(_graph, from)).Select(x => x.Element);
		}

		private float[,] CostMap(TheDungeon gen) {
			float[,] costMap = new float[gen.Size.x, gen.Size.y];

			for (int y = 0; y < gen.Size.y; y++) {
				for (int x = 0; x < gen.Size.x; x++) {
					costMap[x, y] = _costCallback(gen, x, y);
				}
			}
			return costMap;
		}

		public static CostForTile UniformCostWalkablePather() {
			return (TheDungeon gen, int x, int y) => {
				if (!gen.WalkableAt(x, y)) return -1;
				return 1;
			};
		}

		public static CostForTile UniformCostTerrainSpecificWalkablePather(TerrainSO requiredTerrain) {
			return (TheDungeon gen, int x, int y) => {
				TileSpec tile = gen.GetTileSpec(x, y);
				if (!tile.Walkable) return -1;
				if (requiredTerrain != null && tile.Terrain != requiredTerrain) return -1;
				return 1;
			};
		}
	}
}