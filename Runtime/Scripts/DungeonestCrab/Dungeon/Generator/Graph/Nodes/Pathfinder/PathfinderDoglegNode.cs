using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Pathfinding/Pather: Dogleg")]
    public class PathfinderDoglegNode : PathfinderProviderNode {

        public override IPathFinder GetPathFinder() {
            return new PathfinderDogleg();
        }
    }
}