using UnityEngine;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Definitions/Bounds/Inset (Uniform Percent)")]
    public class BoundsInsetPercentUniformNode : BoundsProviderNode {
        [Range(0, 1)] public float Inset = 0.1f;

        public override Bounds GetBounds() => new InsetPercentBounds(Inset);
    }
}
