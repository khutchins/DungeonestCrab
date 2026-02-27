using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Definitions/Bounds/Inset (HV Fixed)")]
    public class BoundsInsetHVNode : BoundsProviderNode {
        public int Horizontal = 1;
        public int Vertical = 1;

        public override Bounds GetBounds() => new InsetBounds(Horizontal, Vertical);
    }
}
