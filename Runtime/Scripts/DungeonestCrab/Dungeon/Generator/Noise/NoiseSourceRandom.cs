using System.Collections;
using System.Collections.Generic;
using Pomerandomian;

namespace KH.Noise {
    public class NoiseSourceRandom : INoiseSource {
        private IRandom _random;

		public void SetSeed(int seed) {
			_random = new SystemRandom(seed);
		}

		public float At(float x, float y) {
			return (float) _random.NextDouble();
		}
	}
}