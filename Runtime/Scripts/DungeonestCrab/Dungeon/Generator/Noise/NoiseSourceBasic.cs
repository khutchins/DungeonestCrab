using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pomerandomian;

namespace KH.Noise {
    public class NoiseSourceBasic : INoiseSource {
        private readonly FastNoiseLite.NoiseType _type;
        private readonly float _frequency;
        private readonly int _octaves;
        private readonly float _lacunarity;
        private readonly float _gain;

        private FastNoiseLite _noise;

        public NoiseSourceBasic(FastNoiseLite.NoiseType type, float frequency, int octaves = 1, float lacunarity = 2.0f, float gain = 0.5f) {
            _type = type;
            _frequency = frequency;
            _octaves = octaves;
            _lacunarity = lacunarity;
            _gain = gain;
        }

        public void SetSeed(int seed) {
            _noise = new FastNoiseLite(seed);
            _noise.SetNoiseType(_type);
            _noise.SetFrequency(_frequency);

            if (_octaves > 1) {
                _noise.SetFractalType(FastNoiseLite.FractalType.FBm);
                _noise.SetFractalOctaves(_octaves);
                _noise.SetFractalLacunarity(_lacunarity);
                _noise.SetFractalGain(_gain);
            }
        }

        public float At(float x, float y) {
            // FastNoiseLite returns -1 to 1, so we have to map it.
            return (_noise.GetNoise(x, y) + 1f) / 2f;
        }
    }
}