using DungeonestCrab.Dungeon.Generator;
using DungeonestCrab.Dungeon.Generator.Graph;
using Pomerandomian;
using UnityEngine;
using static XNode.Node;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Pathfinding/Solvers/Drunkard Walk")]
    public class PathfinderDrunkardNode : PathfinderProviderNode {
        [Range(0, 1)] public float BiasToTarget = 0.5f;
        public int MaxIterations = 5000;

        public override IPathFinder GetPathFinder() {
            return new PathfinderDrunkard(BiasToTarget, MaxIterations);
        }
    }
}