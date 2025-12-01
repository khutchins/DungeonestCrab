using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Actions/Add Entities (Count)")]
    public class AddEntityCountNode : EntityPassNode {
        public int MinCount = 1;
        public int TargetCount = 5;
        [Range(0, 1)] public float Chance = 1f;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            EntitySource source = GetEntitySource();
            return new AddEntity(source, GetMatcher(), AvoidBlockingPath, MinCount, TargetCount, Chance)
                .Modify(dungeon, random);
        }
    }
}