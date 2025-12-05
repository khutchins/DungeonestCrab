using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
    public class ConnectorRandom : IRegionConnector {
        private readonly float _extraConnectionChance;

        public ConnectorRandom(float extraConnectionChance = 0f) {
            _extraConnectionChance = extraConnectionChance;
        }

        public IEnumerable<ConnectionRequest> GetConnections(TheDungeon dungeon, int[,] regionMap, int maxRegion, IRandom rand) {
            Dictionary<int, List<Vector2Int>> regionTiles = new Dictionary<int, List<Vector2Int>>();
            for (int i = 0; i <= maxRegion; i++) regionTiles[i] = new List<Vector2Int>();

            for (int y = 0; y < dungeon.Size.y; y++) {
                for (int x = 0; x < dungeon.Size.x; x++) {
                    int r = regionMap[y, x];
                    if (r >= 0) regionTiles[r].Add(new Vector2Int(x, y));
                }
            }

            List<int> availableRegions = regionTiles.Keys.Where(k => regionTiles[k].Count > 0).ToList();
            List<int> pool = new List<int>(availableRegions);

            while (pool.Count > 1) {
                pool = pool.Shuffle(rand).ToList();
                int rA = pool[0];
                int rB = pool[1];

                yield return new ConnectionRequest {
                    From = rand.From(regionTiles[rA]),
                    To = rand.From(regionTiles[rB]),
                    FromRegionID = rA,
                    ToRegionID = rB
                };

                // Extra connections occur from just not removing the connection from the pool.
                if (!rand.WithPercentChance(_extraConnectionChance)) {
                    pool.RemoveAt(0);
                }
            }
        }
    }
}