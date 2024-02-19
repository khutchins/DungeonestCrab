using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KH.Noise {
	/// <summary>
	/// Noise source that performs operations on child noise sources.
	/// The D stands for derived.
	/// </summary>
    public abstract class IDNoiseSource : INoiseSource {

		protected readonly INoiseSource[] _sources;

		public IDNoiseSource(params INoiseSource[] sources) {
			_sources = sources;
		}

		public void SetSeed(int seed) {
			foreach (INoiseSource source in _sources) {
				source.SetSeed(seed);
			}
		}

		public abstract float At(float x, float y);
	}
}