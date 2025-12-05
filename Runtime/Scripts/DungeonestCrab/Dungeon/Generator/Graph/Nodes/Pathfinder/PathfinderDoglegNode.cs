namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Pathfinding/Pather: Dogleg")]
    public class PathfinderOrthogonalNode : PathfinderProviderNode {
        public override IPathFinder GetPathFinder() {
            return new PathfinderDogLeg();
        }
    }
}