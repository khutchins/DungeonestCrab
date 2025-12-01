using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Bounds/Inset (Percent)")]
    public class BoundsInsetPercentNode : BoundsProviderNode {
        [Range(0, 1)] public float Left = 0.1f;
        [Range(0, 1)] public float Right = 0.1f;
        [Range(0, 1)] public float Top = 0.1f;
        [Range(0, 1)] public float Bottom = 0.1f;

        public override Bounds GetBounds() => new InsetPercentBounds(Left, Right, Top, Bottom);
    }
}