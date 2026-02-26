using DungeonestCrab.Dungeon;
using KH.Noise;
using UnityEngine;
using static XNode.Node;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Sources/Noise")]
    public class SourceNoiseNode : SourceProviderNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public NoiseConnection NoiseInput;
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public NoiseModifierConnection ModifierInput;
        public Tile TileToSet = Tile.Floor;
        [Range(0, 1)] public float Threshold = 0.5f;

        public override ISource GetSource() {
            INoiseSource noise = GetInputValue<INoiseSource>("NoiseInput", null);
            INoiseModifier modifier = GetInputValue<INoiseModifier>("ModifierInput", null);
            // Default noise if not connected
            if (noise == null) noise = new NoiseSourcePerlin(0.1f);
            return new SourceNoise(TileToSet, Threshold, noise, modifier);
        }
    }
}