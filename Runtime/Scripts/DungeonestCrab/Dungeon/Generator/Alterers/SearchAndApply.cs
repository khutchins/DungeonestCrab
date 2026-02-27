using Pomerandomian;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    /// <summary>
    /// Searches for tiles matching specific criteria and applies an action to tiles within a given radius.
    /// </summary>
    public class SearchAndApply : IAlterer {

        readonly IMatcher _searchCriteria;
        readonly IMatcher _targetCriteria;
        readonly int _searchRadius;
        readonly ITileAction _action;

        public SearchAndApply(IMatcher searchCriteria, IMatcher targetCriteria, int searchRadius, ITileAction action) {
            _searchCriteria = searchCriteria;
            _targetCriteria = targetCriteria;
            _searchRadius = searchRadius;
            _action = action;
        }

        public bool Modify(TheDungeon dungeon, IRandom rand) {
            if (_action == null) return true;

            var affectedTiles = new HashSet<TileSpec>();
            
            // Find all tiles that match our search criteria
            foreach (TileSpec origin in dungeon.AllTiles()) {
                if (_searchCriteria.Matches(origin)) {
                    
                    // Check all tiles within the search radius
                    for (int x = -_searchRadius; x <= _searchRadius; x++) {
                        for (int y = -_searchRadius; y <= _searchRadius; y++) {
                            var neighbor = dungeon.GetTileSpecSafe(origin.Coords.x + x, origin.Coords.y + y);
                            
                            // Only collect those that match our target criteria
                            if (neighbor != null && !neighbor.Immutable && (_targetCriteria == null || _targetCriteria.Matches(neighbor))) {
                                affectedTiles.Add(neighbor);
                            }
                        }
                    }
                }
            }

            // Apply the action to all collected tiles
            foreach (var tile in affectedTiles) {
                _action.Apply(tile, rand);
            }
            return true;
        }
    }
}
