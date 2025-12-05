using System.Collections.Generic;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
    public class PathfinderDogleg : IPathFinder {

        public void Init(IRandom rand) { }
        public IEnumerable<Vector2Int> FindPath(TheDungeon dungeon, Vector2Int start, Vector2Int end, IRandom rand) {
            bool xFirst = rand.NextBool();
            List<Vector2Int> path = TryPath(dungeon, start, end, xFirst);
            if (path != null) return path;

            // If the first path was invalid (crossed protected node), try the other one.
            path = TryPath(dungeon, start, end, !xFirst);
            return path;
        }

        private List<Vector2Int> TryPath(TheDungeon dungeon, Vector2Int start, Vector2Int end, bool xFirst) {
            List<Vector2Int> path = new List<Vector2Int>();
            Vector2Int current = start;
            path.Add(current);

            if (xFirst) {
                // Move X
                while (current.x != end.x) {
                    current.x += System.Math.Sign(end.x - current.x);
                    if (IsBlocked(dungeon, current)) return null;
                    path.Add(current);
                }
                // Move Y
                while (current.y != end.y) {
                    current.y += System.Math.Sign(end.y - current.y);
                    if (IsBlocked(dungeon, current)) return null;
                    path.Add(current);
                }
            } else {
                // Move Y
                while (current.y != end.y) {
                    current.y += System.Math.Sign(end.y - current.y);
                    if (IsBlocked(dungeon, current)) return null;
                    path.Add(current);
                }
                // Move X
                while (current.x != end.x) {
                    current.x += System.Math.Sign(end.x - current.x);
                    if (IsBlocked(dungeon, current)) return null;
                    path.Add(current);
                }
            }
            return path;
        }

        private bool IsBlocked(TheDungeon dungeon, Vector2Int pt) {
            if (!dungeon.Contains(pt)) return true;
            // Immutable tiles should not be edited.
            return dungeon.GetTileSpec(pt).Immutable;
        }
    }
}