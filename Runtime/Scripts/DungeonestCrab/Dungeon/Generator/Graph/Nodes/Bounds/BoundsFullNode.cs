namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Definitions/Bounds/Full")]
    public class BoundsFullNode : BoundsProviderNode {
        public override Bounds GetBounds() => new FullBounds();
    }
}