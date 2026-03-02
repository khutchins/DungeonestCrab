 using DungeonestCrab.Dungeon.Generator;
using DungeonestCrab.Dungeon.Generator.Graph;
using Pomerandomian;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Nodes {
    [CreateNodeMenu("Dungeon/Passes/Topology/Remove Dead Ends")]
    public class DeadEndRemoverNode : DungeonPassNode {

        public int MaxDeadEndLength = 4;
        
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public MatcherConnection Filter;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            IMatcher filter = GetInputValue<IMatcher>("Filter", null);
            return new DeadEndRemover(MaxDeadEndLength, filter).Modify(dungeon, random);
        }
    }
}
