using UnityEngine;
using KH.Noise;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Noise/Basic (FastNoiseLite)")]
    public class BasicNoiseNode : NoiseProviderNode {
        public FastNoiseLite.NoiseType Type = FastNoiseLite.NoiseType.OpenSimplex2;
        public float Frequency = 0.05f;

        [Header("Fractal (Roughness)")]
        [Tooltip("1 = Smooth blobs. 3-5 = Rocky/Cloudy details.")]
        [Range(1, 8)] public int Octaves = 1;
        [Tooltip("How much detail is added per octave.")]
        public float Lacunarity = 2.0f;
        [Tooltip("How strong the detail is.")]
        public float Gain = 0.5f;

        public override INoiseSource CreateNoiseSource() {
            return new NoiseSourceBasic(Type, Frequency, Octaves, Lacunarity, Gain);
        }
    }
}