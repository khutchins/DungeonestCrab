using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KH.Noise {
	/// <summary>
	/// Adds all the provided sources together. Be aware:
	/// doing this can result in "too bright" noise if you're
	/// not careful.
	/// </summary>
	public class DNoiseSourceAdd : IDNoiseSource {
		public override float At(float x, float y) {
			float val = 0;
			foreach (INoiseSource source in _sources) {
				val += source.At(x, y);
			}
			return val;
		}
	}
}