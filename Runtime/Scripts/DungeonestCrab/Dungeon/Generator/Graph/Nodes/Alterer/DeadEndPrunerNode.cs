using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Passes/Topology/Prune Dead Ends")]
    public class DeadEndPrunerNode : DungeonPassNode {
        protected override bool ApplyNodeLogic(TheDungeon dungeon, ISeededRandom random) {
            return new DeadEndPruner().Modify(dungeon, random);
        }
    }
}