using DungeonestCrab.Dungeon;
using KH.Noise;
using UnityEngine;
using static XNode.Node;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Sources/Noise")]
    public class SourceNoiseNode : SourceProviderNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public NoiseConnection NoiseInput;
        public Tile TileToSet = Tile.Floor;
        [Range(0, 1)] public float Threshold = 0.5f;

        public override ISource GetSource() {
            INoiseSource noise = GetInputValue<INoiseSource>("NoiseInput", null);
            // Default noise if not connected
            if (noise == null) noise = new NoiseSourcePerlin(0.1f);
            return new SourceNoise(TileToSet, Threshold, noise, null);
        }
    }
}