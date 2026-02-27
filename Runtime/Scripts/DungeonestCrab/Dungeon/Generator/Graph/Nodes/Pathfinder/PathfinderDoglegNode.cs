using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Pathfinding/Solvers/Dogleg")]
    public class PathfinderDoglegNode : PathfinderProviderNode {

        public override IPathFinder GetPathFinder() {
            return new PathfinderDogleg();
        }
    }
}