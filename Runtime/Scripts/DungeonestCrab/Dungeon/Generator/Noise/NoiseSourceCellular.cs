using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KH.Noise {
	public class NoiseSourceCellular : INoiseSource {

		private readonly float _frequency;
		private readonly float _jitter;
		private readonly FastNoiseLite.CellularDistanceFunction _distanceFunction;
		private readonly FastNoiseLite.CellularReturnType _returnType;
		private FastNoiseLite _noise;

		public NoiseSourceCellular(float frequency, float jitter, FastNoiseLite.CellularDistanceFunction distanceFunction, FastNoiseLite.CellularReturnType returnType) {
			_frequency = frequency;
			_jitter = jitter;
			_distanceFunction = distanceFunction;
			_returnType = returnType;
		}

		public void SetSeed(int seed) {
			_noise = new FastNoiseLite(seed);
			_noise.SetFrequency(_frequency);
			_noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
			_noise.SetCellularJitter(_jitter);
			_noise.SetCellularDistanceFunction(_distanceFunction);
			_noise.SetCellularReturnType(_returnType);
		}

		public float At(float x, float y) {
			return (_noise.GetNoise(x, y) + 1) / 2f;
		}
    }
}