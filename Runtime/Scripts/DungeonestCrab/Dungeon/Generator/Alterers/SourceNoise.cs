using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;
using KH.Noise;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator {

	public interface INoiseModifier {
		float Modifier(AppliedBounds bounds, int x, int y);
	}

	public class ConstantModifier : INoiseModifier {
		public float modifier;

		public ConstantModifier(float modifier = 1F) {
			this.modifier = modifier;
		}

		public float Modifier(AppliedBounds bounds, int x, int y) {
			return modifier;
		}
	}

	public class CircleModifier : INoiseModifier {
		public float startRadius;
		public float endRadius;
		public float minMod;
		public float maxMod;

		/// <summary>
		/// Create a circle modifier with the given attributes.
		/// </summary>
		/// <param name="startRadius">The minimum radius, below which minMod will be returned.</param>
		/// <param name="endRadius">The maximum radius, above which maxMod will be returned. Between the start and end radius, the value will be linearly interpolated between maxMod and minMod.</param>
		/// <param name="minMod">The modification amount past the end radius, where a value of 0 subtracts 0 from the computed noise, and 1 subtracting 1.</param>
		/// <param name="maxMod">The modification amount within the start radius, where a value of 0 subtracts 0 from the computed noise, and 1 subtracting 1.</param>
		public CircleModifier(float startRadius, float endRadius, float minMod = 0, float maxMod = 1) {
			this.startRadius = startRadius;
			this.endRadius = endRadius;
			this.minMod = minMod;
			this.maxMod = maxMod;
		}

		public float Modifier(AppliedBounds bounds, int x, int y) {
			float cx = bounds.w / 2F;
			float cy = bounds.h / 2F;

			float dist = (cx - x) * (cx - x) + (cy - y) * (cy - y);
			float startRad2 = startRadius * startRadius;
			if (dist < startRad2) return minMod;
			float endRad2 = endRadius * endRadius;
			if (dist < endRad2) return maxMod;
			dist -= startRad2;
			return dist / endRad2 * (maxMod - minMod) + minMod;
		}
	}

	public class InsetSquareNoiseModifier : INoiseModifier {
		public int insetWidth;
		public int gradientLength;
		public float minMod;
		public float maxMod;

		public InsetSquareNoiseModifier(int inset, int gradientLength, float minMod = 0, float maxMod = 1) {
			insetWidth = inset;
			this.gradientLength = gradientLength;
			this.minMod = minMod;
			this.maxMod = maxMod;
		}

		public float Modifier(AppliedBounds bounds, int x, int y) {
			int minDist = Mathf.Min(x, y, bounds.w - x, bounds.h - y);
			if (minDist < insetWidth) return minMod;
			minDist -= insetWidth;
			if (minDist < gradientLength) return minDist / gradientLength * (maxMod - minMod) + minMod;
			return maxMod;
		}
	}

	public class SourceNoise : ISource {
		private readonly float minNoiseThreshold;
		private readonly INoiseSource source;
		private readonly INoiseModifier modifier;

		public SourceNoise(Tile tileToSet, float minNoiseThreshold, INoiseSource source = null, INoiseModifier modifier = null) : base(tileToSet) {
			this.minNoiseThreshold = minNoiseThreshold;
			this.modifier = modifier;
			this.source = source ?? new NoiseSourcePerlin(0.015f);
		}

		public override void Generate(Stamp stamp, IRandom rand) {
			int w = stamp.W;
			int h = stamp.H;

			source.SetSeed(rand.Next(int.MaxValue));

			AppliedBounds bounds = new AppliedBounds(0, 0, w - 2, h - 2);

			for (int iy = 1; iy < h - 1; iy++) {
				for (int ix = 1; ix < w - 1; ix++) {
					float noise = source.At(ix, iy);
					if (modifier != null) noise -= modifier.Modifier(bounds, ix - 1, iy - 1);

					if (noise > minNoiseThreshold) {
						stamp.MaybeSetAt(ix, iy, _tileToSet);
					}
				}
			}
		}
	}
}