using System.Collections;
using System.Collections.Generic;
using Pomerandomian;

namespace KH.Noise {
    public class NoiseSourceRandom : INoiseSource {
        private ISeededRandom _random;

		public void SetSeed(int seed) {
			_random = new Xoshiro256PpRandom(seed);
		}

		public float At(float x, float y) {
			return (float) _random.NextDouble();
		}
	}
}