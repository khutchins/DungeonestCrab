using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pomerandomian;

namespace KH.Noise {
	public class NoiseSourcePerlin : INoiseSource {

		public readonly float SampleDistance = 0.015f;
		private Vector2 _offset = Vector2.one;

		public NoiseSourcePerlin(float sampleDistance) {
			SampleDistance = sampleDistance;
		}

		public void SetSeed(int seed) {
			IRandom rand = new SystemRandom(seed);
			_offset = new Vector2((float)(rand.NextDouble() * 1000000), (float)(rand.NextDouble() * 1000000));
		}

		public float At(float x, float y) {
			return Mathf.PerlinNoise(x * SampleDistance + _offset.x, y * SampleDistance + _offset.x);
		}
	}
}