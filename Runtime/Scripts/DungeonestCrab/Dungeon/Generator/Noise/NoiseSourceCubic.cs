using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KH.Noise {
	// Source: https://github.com/jobtalle/CubicNoise
	public class NoiseSourceCubic : INoiseSource {

		private static readonly int RND_A = 134775813;
		private static readonly int RND_B = 1103515245;

		private int _seed;
		private readonly float _octaves;
		private readonly float _falloff;
		private readonly float _period;

		private readonly float _baseAmplitude; 

		/// <summary>
		/// A generator for cubic noise.
		/// </summary>
		/// <param name="octaves">How many octaves to use. More octaves cause a noisier result. Normal range: 1-10</param>
		/// <param name="falloff">Falloff parameter to use. Affects how much influence higher octaves have on the final image. Normal range: 0.25-16</param>
		/// <param name="period">Period of the range. Higher values will zoom in. Normal range: 1-256</param>
		public NoiseSourceCubic(int octaves, float falloff, int period) {
			_octaves = octaves;
			_falloff = falloff == 0 ? 0.001f : falloff;
			_period = period == 0 ? 1 : period;

			if (_falloff - 1 == 0) {
				_baseAmplitude = 1 / _octaves / _falloff;
			} else {
				_baseAmplitude = (((_falloff - 1) * Mathf.Pow(_falloff, _octaves)) / (Mathf.Pow(_falloff, _octaves) - 1)) / _falloff;
			}
		}

		private static float sample(int seed, int octave, float x) {
			int xi = (int)Mathf.Floor(x / octave);
			float lerp = x / octave - xi;

			return interpolate(
				randomize(seed, xi - 1, 0),
				randomize(seed, xi,     0),
				randomize(seed, xi + 1, 0),
				randomize(seed, xi + 2, 0),
				lerp) * 0.5f + 0.25f;
		}

		private static float sample(int seed, float octave, float x, float y) {
			int xi = (int)Mathf.Floor(x / octave);
			float lerpx = x / octave - xi;
			int yi = (int)Mathf.Floor(y / octave);
			float lerpy = y / octave - yi;

			float[] xSamples = new float[4];

			for (int i = 0; i < 4; ++i)
				xSamples[i] = interpolate(
						randomize(seed, xi - 1, yi - 1 + i),
						randomize(seed, xi,     yi - 1 + i),
						randomize(seed, xi + 1, yi - 1 + i),
						randomize(seed, xi + 2, yi - 1 + i),
						lerpx);

			return interpolate(xSamples[0], xSamples[1], xSamples[2], xSamples[3], lerpy) * 0.666666f + 0.166666f;
		}

		private static float randomize(int seed, int x, int y) {
			return (float)((((x ^ y) * RND_A) ^ (seed + x)) * (((RND_B * x) << 16) ^ (RND_B * y) - RND_A)) / int.MaxValue;
		}

		private static int tile(int coordinate, int period) {
			return coordinate % period;
		}

		private static float interpolate(float a, float b, float c, float d, float x) {
			float p = (d - c) - (a - b);
			return x * (x * (x * p + ((a - b) - p)) + (c - a)) + b;
		}

		public void SetSeed(int seed) {
			_seed = seed;
		}

		public float At(float x, float y) {
			float value = 0;

			float amplitude = _baseAmplitude;
			float period = _period;
			for (int i = 0; i < _octaves; i++) {
				value += sample(_seed + i, _period / (i + 1), x, y) * amplitude;
				period /= 2;
				amplitude /= _falloff;
			}

			return value;
		}
    }
}