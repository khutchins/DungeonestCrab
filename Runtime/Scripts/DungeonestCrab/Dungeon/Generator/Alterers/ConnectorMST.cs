using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;
using KH.Graph;

namespace DungeonestCrab.Dungeon.Generator {
    public class ConnectorMST : IRegionConnector {
        private readonly bool _useCentroids;
        private readonly float _loopChance;

        public ConnectorMST(bool useCentroids = true, float loopChance = 0f) {
            _useCentroids = useCentroids;
            _loopChance = loopChance;
        }

        public IEnumerable<ConnectionRequest> GetConnections(TheDungeon dungeon, int[,] regionMap, int maxRegion, IRandom rand) {
            var regionPoints = new Dictionary<int, List<Vector2Int>>();
            for (int i = 0; i <= maxRegion; i++) regionPoints[i] = new List<Vector2Int>();

            for (int y = 0; y < dungeon.Size.y; y++) {
                for (int x = 0; x < dungeon.Size.x; x++) {
                    int r = regionMap[y, x];
                    if (r >= 0) regionPoints[r].Add(new Vector2Int(x, y));
                }
            }

            var anchors = new List<Vector2Int>();
            var posToRegion = new Dictionary<Vector2Int, int>();

            foreach (var kvp in regionPoints) {
                if (kvp.Value.Count == 0) continue;
                Vector2Int anchor;

                if (_useCentroids) {
                    long sumX = 0, sumY = 0;
                    foreach (var p in kvp.Value) { sumX += p.x; sumY += p.y; }
                    Vector2 centroid = new Vector2(sumX / (float)kvp.Value.Count, sumY / (float)kvp.Value.Count);
                    anchor = kvp.Value.OrderBy(p => (new Vector2(p.x, p.y) - centroid).sqrMagnitude).First();
                } else {
                    anchor = rand.From(kvp.Value);
                }

                anchors.Add(anchor);
                if (!posToRegion.ContainsKey(anchor)) posToRegion[anchor] = kvp.Key;
            }

            if (anchors.Count < 2) yield break;

            UndirectedGraph<Vector2Int> graph = Vector2IntCoordExtensions.CliqueGraph(anchors);

            var mstEdges = graph.MinimumSpanningTree();
            var mstEdgeSet = new HashSet<UndirectedGraph<Vector2Int>.Edge>(mstEdges);

            foreach (var edge in mstEdges) {
                yield return new ConnectionRequest {
                    From = edge.N1.Element,
                    To = edge.N2.Element,
                    FromRegionID = posToRegion[edge.N1.Element],
                    ToRegionID = posToRegion[edge.N2.Element]
                };
            }

            if (_loopChance > 0) {
                var allEdges = new List<UndirectedGraph<Vector2Int>.Edge>();
                var seenEdges = new HashSet<UndirectedGraph<Vector2Int>.Edge>();

                foreach (var node in graph.Nodes) {
                    foreach (var edge in node.Edges) {
                        if (seenEdges.Add(edge)) {
                            allEdges.Add(edge);
                        }
                    }
                }

                // Prefer local neighbors.
                allEdges.Sort((a, b) => a.Cost.CompareTo(b.Cost));

                int candidateLimit = anchors.Count * 3;
                int checkedCount = 0;

                foreach (var edge in allEdges) {
                    // Skip already-visited paths.
                    if (mstEdgeSet.Contains(edge)) continue;

                    // Stop looking if we've checked the nearest neighbors.
                    if (checkedCount++ > candidateLimit) break;

                    if (rand.WithPercentChance(_loopChance)) {
                        yield return new ConnectionRequest {
                            From = edge.N1.Element,
                            To = edge.N2.Element,
                            FromRegionID = posToRegion[edge.N1.Element],
                            ToRegionID = posToRegion[edge.N2.Element]
                        };
                    }
                }
            }
        }
    }
}