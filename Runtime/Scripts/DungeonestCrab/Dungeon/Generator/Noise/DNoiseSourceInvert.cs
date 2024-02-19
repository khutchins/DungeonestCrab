using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KH.Noise {
	/// <summary>
	/// Inverts the provided noise source (i.e. returns 1 - source).
	/// </summary>
	public class DNoiseSourceInvert : IDNoiseSource {

		public DNoiseSourceInvert(INoiseSource sourceToInvert) : base(sourceToInvert) { }

		public override float At(float x, float y) {
			return 1 - _sources[0].At(x, y);
		}
	}
}