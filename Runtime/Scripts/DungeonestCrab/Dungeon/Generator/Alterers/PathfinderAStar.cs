using DungeonestCrab.Dungeon.Generator.Graph;
using Pomerandomian;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    public class PathfinderAStar : IPathFinder {
        public delegate float CostFunc(TheDungeon d, int x, int y);
        private readonly Graph.ITileCostProvider _costProvider;

        public PathfinderAStar(ITileCostProvider costProvider) {
            _costProvider = costProvider;
        }

        public void Init(IRandom rand) {
            _costProvider.Init(rand);
        }

        public IEnumerable<Vector2Int> FindPath(TheDungeon dungeon, Vector2Int start, Vector2Int end, IRandom rand) {
            float[,] costs = new float[dungeon.Size.x, dungeon.Size.y];
            for (int x = 0; x < dungeon.Size.x; x++) {
                for (int y = 0; y < dungeon.Size.y; y++) {
                    costs[x, y] = _costProvider.GetCost(dungeon, x, y);
                }
            }

            var graph = GraphExtensions.FromCostMap(costs);
            var startNode = graph.Nodes.FirstOrDefault(n => n.Element == start);
            var endNode = graph.Nodes.FirstOrDefault(n => n.Element == end);

            if (startNode == null || endNode == null) return null;

            var pathNodes = graph.FindPath(startNode, endNode, (a, b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y));

            if (pathNodes == null) return null;
            return pathNodes.Select(n => n.Element);
        }
    }
}