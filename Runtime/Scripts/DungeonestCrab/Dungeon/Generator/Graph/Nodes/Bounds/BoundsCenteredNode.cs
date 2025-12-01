using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Bounds/Centered (Percent)")]
    public class BoundsCenteredNode : BoundsProviderNode {
        [Range(0, 1)] public float CenterX = 0.5f;
        [Range(0, 1)] public float CenterY = 0.5f;
        [Range(0, 1)] public float Width = 0.5f;
        [Range(0, 1)] public float Height = 0.5f;

        public override Bounds GetBounds() => new CenteredBounds(CenterX, CenterY, Width, Height);
    }
}