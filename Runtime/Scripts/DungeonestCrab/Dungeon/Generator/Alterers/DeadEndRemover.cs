using DungeonestCrab.Dungeon;
using Pomerandomian;
using System.Collections.Generic;
using System.Linq;

namespace DungeonestCrab.Dungeon.Generator {
    [System.Serializable]
    public class DeadEndRemover : IAlterer {
        public int MaxDeadEndLength = 4;
        private IMatcher _filter;

        public DeadEndRemover() {}

        public DeadEndRemover(int maxDeadEndLength, IMatcher filter) {
            MaxDeadEndLength = maxDeadEndLength;
            _filter = filter;
        }

        public bool Modify(TheDungeon dungeon, IRandom rand) {
            List<TileSpec> deadEnds = DeadEndPruner.GetDeadEnds(dungeon).ToList();
            bool modificationsMade = false;
            
            List<List<TileSpec>> pathsToPrune = new List<List<TileSpec>>();

            foreach (var tip in deadEnds) {
                if (tip.Tile != Tile.Floor) continue;

                List<TileSpec> currentPath = new List<TileSpec>();
                TileSpec currentTile = tip;

                while (currentTile != null) {
                    if (_filter != null && !_filter.Matches(currentTile)) {
                        break;
                    }
                    
                    var walkables = dungeon.WalkableAdjacencies(currentTile.Coords).ToList();
                    
                    if (walkables.Count > 2) {
                        break;
                    }

                    currentPath.Add(currentTile);

                    TileSpec nextTile = walkables.FirstOrDefault(t => !currentPath.Contains(t));
                    
                    if (nextTile == null) {
                        break;
                    }

                    currentTile = nextTile;
                }

                if (currentPath.Count > MaxDeadEndLength) {
                    pathsToPrune.Add(currentPath);
                }
            }
            
            foreach (var path in pathsToPrune) {
                int tilesToRemove = path.Count - MaxDeadEndLength;
                
                for (int i = 0; i < tilesToRemove; i++) {
                    if (path[i].Tile == Tile.Floor) {
                        path[i].Tile = Tile.Wall;
                        modificationsMade = true;
                    }
                }
            }

            return modificationsMade;
        }
    }
}
