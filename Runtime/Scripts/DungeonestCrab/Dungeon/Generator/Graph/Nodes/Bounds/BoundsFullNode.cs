namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Bounds/Full")]
    public class BoundsFullNode : BoundsProviderNode {
        public override Bounds GetBounds() => new FullBounds();
    }
}