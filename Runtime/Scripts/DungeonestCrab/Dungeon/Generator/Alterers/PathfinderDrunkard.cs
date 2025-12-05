using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
    public class PathfinderDrunkard : IPathFinder {
        private readonly float _bias;
        private readonly int _maxIterations;

        public PathfinderDrunkard(float biasToTarget, int maxIterations = 5000) {
            _bias = Mathf.Clamp01(biasToTarget);
            _maxIterations = maxIterations;
        }

        public void Init(IRandom rand) { }
        public IEnumerable<Vector2Int> FindPath(TheDungeon dungeon, Vector2Int start, Vector2Int end, IRandom rand) {
            Stack<Vector2Int> pathStack = new Stack<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

            pathStack.Push(start);
            visited.Add(start);

            int iterations = 0;

            while (pathStack.Count > 0) {
                if (iterations++ > _maxIterations) {
                    Debug.LogWarning("PathfinderDrunkard timed out.");
                    return null;
                }

                Vector2Int current = pathStack.Peek();

                if (current == end) {
                    return pathStack.Reverse().ToList();
                }

                var validNeighbors = current.Adjacencies1Away()
                    .Where(n => dungeon.Contains(n) && !dungeon.GetTileSpec(n).Immutable)
                    .Where(n => !visited.Contains(n))
                    .ToList();

                // Short circuit if any neighbors have the end (we're not THAT drunk).
                if (validNeighbors.Contains(end)) {
                    pathStack.Push(end);
                    continue;
                }

                if (validNeighbors.Count > 0) {
                    Vector2Int next;

                    if (rand.WithPercentChance(_bias)) {
                        next = validNeighbors.OrderBy(n => (n - end).sqrMagnitude).First();
                    } else {
                        next = rand.From(validNeighbors);
                    }

                    pathStack.Push(next);
                    visited.Add(next);
                } else {
                    pathStack.Pop();
                }
            }

            return null;
        }
    }
}