using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;
using KH.Noise;

namespace DungeonestCrab.Dungeon.Generator {
    public struct ConnectionRequest {
        public Vector2Int From;
        public Vector2Int To;
        public int FromRegionID;
        public int ToRegionID;
    }

    public interface IRegionConnector {
        IEnumerable<ConnectionRequest> GetConnections(TheDungeon dungeon, int[,] regionMap, int maxRegion, IRandom rand);
    }

    public interface IPathFinder {
        IEnumerable<Vector2Int> FindPath(TheDungeon dungeon, Vector2Int start, Vector2Int end, IRandom rand);
    }

    public interface ITileCarver {
        void Carve(TheDungeon dungeon, IEnumerable<Vector2Int> path, IRandom rand);
    }

    public class ModularCombiner : IAlterer {
        private readonly IRegionConnector _connector;
        private readonly IPathFinder _pathFinder;
        private readonly ITileCarver _carver;

        public ModularCombiner(IRegionConnector connector, IPathFinder pathFinder, ITileCarver carver) {
            _connector = connector;
            _pathFinder = pathFinder;
            _carver = carver;
        }

        public bool Modify(TheDungeon dungeon, IRandom rand) {
            int maxRegion;
            int[,] regionMap = dungeon.ComputeRegions(out maxRegion);

            IEnumerable<ConnectionRequest> requests = _connector.GetConnections(dungeon, regionMap, maxRegion, rand);

            foreach (var req in requests) {
                IEnumerable<Vector2Int> path = _pathFinder.FindPath(dungeon, req.From, req.To, rand);

                if (path != null && path.Any()) {
                    _carver.Carve(dungeon, path, rand);
                } else {
                    Debug.LogWarning($"Failed to find path between {req.From} and {req.To}");
                }
            }

            return true;
        }
    }
}