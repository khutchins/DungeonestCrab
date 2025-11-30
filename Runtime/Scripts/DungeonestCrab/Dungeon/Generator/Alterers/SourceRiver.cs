using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pomerandomian;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator {
	public class SourceRiver : ISource {
		class RiverPoint {
			public readonly float x;
			public readonly float y;
			public readonly float radius;

			public RiverPoint(float x, float y, float radius) {
				this.x = x;
				this.y = y;
				this.radius = radius;
			}
		}

		[Flags]
		public enum Sides {
			Top = 1 << 0,
			Left = 1 << 1,
			Right = 1 << 2,
			Bottom = 1 << 3
		}

		private readonly float _minWidth;
		private readonly float _maxWidth;
		private readonly List<Sides> _startSides;
		private readonly List<Sides> _endSides;

		public SourceRiver(Tile tile = Tile.Wall, float minWidth = 0.5F, float maxWidth = 1.5F, 
			Sides startSides = Sides.Top | Sides.Left | Sides.Right | Sides.Bottom,
			Sides endSides = Sides.Top | Sides.Left | Sides.Right | Sides.Bottom) : base(tile) {
			_minWidth = minWidth;
			_maxWidth = maxWidth;
			_startSides = SidesForFlags(startSides);
			_endSides = SidesForFlags(endSides);
		}

		private List<Sides> SidesForFlags(Sides sides) {
			List<Sides> allSides = new List<Sides>();

			void addIf(Sides test) {
				if (sides.HasFlag(test)) allSides.Add(test);
			}
			foreach (int val in Enum.GetValues(typeof(Sides))) {
				addIf((Sides)val);
			}
			return allSides;
		}

		private Dir FromSide(Sides side) {
            switch (side) {
                case Sides.Top:
					return Dir.Down;
                case Sides.Left:
					return Dir.Left;
				case Sides.Right:
					return Dir.Right;
				case Sides.Bottom:
					return Dir.Up;
			}
			Debug.LogWarning($"Invalid side: {side}");
			return Dir.Up;
		}

        public override void Generate(Stamp stamp, IRandom rand) {
			int w = stamp.W;
			int h = stamp.H;
			Sides startSide = rand.From(_startSides);
			Dir start = FromSide(startSide);
			List<Sides> remaining = new List<Sides>();
			remaining.AddRange(_endSides);
			remaining.Remove(startSide);
			if (remaining.Count < 1) {
				remaining.Add(startSide == Sides.Top ? Sides.Bottom : Sides.Top);
			}

			Dir end = FromSide(rand.From(remaining));

			RiverPoint startPoint = ControlPoint(start, w, h, rand);
			RiverPoint mid = ControlPoint(w, h, rand);
			RiverPoint endPoint = ControlPoint(end, w, h, rand);
			Displace(stamp, startPoint, mid, rand);
			Displace(stamp, mid, endPoint, rand);
		}

		private void Displace(Stamp stamp, RiverPoint start, RiverPoint dest, IRandom rand) {
			int w = stamp.W;
			int h = stamp.H;
			float wl = dest.x - start.x;
			float hl = dest.y - start.y;

			float len = (float)Math.Sqrt(wl * wl + hl * hl);
			// Subdivide if too long
			if (len > 1F) {
				float x = (start.x + dest.x) / 2F + (float)rand.NextDouble(0, len / 2) - len / 4F;
				float y = (start.y + dest.y) / 2F + (float)rand.NextDouble(0, len / 2) - len / 4F;
				float radius = (start.radius + dest.radius) / 2F;
				RiverPoint mid = new RiverPoint(x, y, radius);
				Displace(stamp, start, mid, rand);
				Displace(stamp, mid, dest, rand);
				return;
			}

			int x1 = Mathf.Clamp(Mathf.FloorToInt(start.x - start.radius), 0, (int)w - 1);
			int x2 = Mathf.Clamp(Mathf.CeilToInt(start.x + start.radius), 0, (int)w - 1);
			int y1 = Mathf.Clamp(Mathf.FloorToInt(start.y - start.radius), 0, (int)h - 1);
			int y2 = Mathf.Clamp(Mathf.CeilToInt(start.y + start.radius), 0, (int)h - 1);

			float r2 = start.radius * start.radius;

			for (int y = y1; y <= y2; y++) {
				for (int x = x1; x <= x2; x++) {
					float xx = start.x - x;
					float yy = start.y - y;
					float lengthSquared = xx * xx + yy * yy;
					if (lengthSquared <= r2) {
						stamp.MaybeSetAt(x, y, _tileToSet);
					}
				}
			}
		}

		private float RandomWidth(IRandom rand) {
			return (float)rand.NextDouble(_minWidth, _maxWidth);
		}

		private RiverPoint ControlPoint(int w, int h, IRandom rand) {
			float x = (float)(rand.NextDouble(w * .25, w * .75));
			float y = (float)(rand.NextDouble(h * .25, h * .75));
			return new RiverPoint(x, y, RandomWidth(rand));
		}

		private RiverPoint ControlPoint(Dir dir, int w, int h, IRandom rand) {
			float x = (float)(rand.NextDouble(w * .25, w * .75));
			float y = (float)(rand.NextDouble(h * .25, h * .75));

			switch (dir) {
				case Dir.Left:
					return new RiverPoint(0, y, RandomWidth(rand));
				case Dir.Right:
					return new RiverPoint(w - 1, y, RandomWidth(rand));
				case Dir.Up:
					return new RiverPoint(x, 0, RandomWidth(rand));
				case Dir.Down:
					return new RiverPoint(x, h - 1, RandomWidth(rand));
			}
			throw new ArgumentException(string.Format("{0} is not a valid dir.", dir));
		}
    }
}