using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Actions/Prune Dead Ends")]
    public class DeadEndPrunerNode : DungeonPassNode {
        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            return new DeadEndPruner().Modify(dungeon, random);
        }
    }
}