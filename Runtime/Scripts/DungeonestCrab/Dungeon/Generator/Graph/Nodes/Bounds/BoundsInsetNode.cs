namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Bounds/Inset (Fixed)")]
    public class BoundsInsetNode : BoundsProviderNode {
        public int Left = 1;
        public int Right = 1;
        public int Top = 1;
        public int Bottom = 1;

        public override Bounds GetBounds() => new InsetBounds(Left, Right, Top, Bottom);
    }
}