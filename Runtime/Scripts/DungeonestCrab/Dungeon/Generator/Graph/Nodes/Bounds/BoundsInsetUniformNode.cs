using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Definitions/Bounds/Inset (Uniform Fixed)")]
    public class BoundsInsetUniformNode : BoundsProviderNode {
        public int Inset = 1;

        public override Bounds GetBounds() => new InsetBounds(Inset);
    }
}
