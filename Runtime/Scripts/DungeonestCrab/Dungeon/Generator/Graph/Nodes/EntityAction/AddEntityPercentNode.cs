using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Actions/Add Entities (Percent)")]
    public class AddEntityPercentNode : EntityPassNode {
        [Range(0, 1)] public float Percent = 0.1f;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            EntitySource source = GetEntitySource();
            if (source == null) return false;

            return new AddEntityPercent(source, GetMatcher(), AvoidBlockingPath, Percent)
                .Modify(dungeon, random);
        }
    }
}