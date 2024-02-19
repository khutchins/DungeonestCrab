using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KH.Noise {
	public class DNoiseSourceAvg : IDNoiseSource {

		private readonly float _percentS1;

		public DNoiseSourceAvg(INoiseSource s1, INoiseSource s2, float percentS1 = 0.5f) : base(s1, s2) {
			_percentS1 = percentS1;
		}

		public override float At(float x, float y) {
			return _sources[0].At(x, y) * _percentS1
				 + _sources[1].At(x, y) * (1 - _percentS1);
		}
	}
}