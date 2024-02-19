using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pomerandomian;

namespace KH.Noise {
	public class NoiseSourceBasic : INoiseSource {

		public enum Type {
			Perlin,
			Simplex2,
			Simplex2S,
		}

		private readonly float _frequency;
		private readonly Type _type;
		private FastNoiseLite _noise;

		public NoiseSourceBasic(float frequency, Type type) {
			_frequency = frequency;
			_type = type;
		}

		public void SetSeed(int seed) {
			_noise = new FastNoiseLite(seed);
			_noise.SetFrequency(_frequency);
			switch (_type) {
				case Type.Perlin:
					_noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
					break;
				case Type.Simplex2:
					_noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
					break;
				case Type.Simplex2S:
					_noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
					break;
			}
		}

		public float At(float x, float y) {
			return (_noise.GetNoise(x, y) + 1) / 2f;
		}
	}
}