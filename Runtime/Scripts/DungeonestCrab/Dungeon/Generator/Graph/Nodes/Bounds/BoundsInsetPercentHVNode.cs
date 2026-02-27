using UnityEngine;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Definitions/Bounds/Inset (HV Percent)")]
    public class BoundsInsetPercentHVNode : BoundsProviderNode {
        [Range(0, 1)] public float Horizontal = 0.1f;
        [Range(0, 1)] public float Vertical = 0.1f;

        public override Bounds GetBounds() => new InsetPercentBounds(Horizontal, Vertical);
    }
}
