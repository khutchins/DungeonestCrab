using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Sources/Stripes")]
    public class SourceStripeNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        [Range(0, 1)] public float Density = 0.2f;
        [Range(0, 1)] public float CurveOdds = 0.1f;
        [Range(0, 1)] public float DeadEndOdds = 0.1f;

        public override ISource GetSource() => new SourceStripe(TileToSet, Density, CurveOdds, DeadEndOdds);
    }
}