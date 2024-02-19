using UnityEngine;
using Pomerandomian;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace DungeonestCrab.Dungeon {

	public class AppliedBounds {
		public readonly int x;
		public readonly int y;
		public readonly int w;
		public readonly int h;

		public AppliedBounds(int x, int y, int w, int h) {
			this.x = x;
			this.y = y;
			this.w = w;
			this.h = h;
		}

		public IEnumerable<Vector2Int> AllTiles(TheDungeon dungeon) {
			for (int iy = y; iy < y + h && iy < dungeon.Size.y; iy++) {
				for (int ix = x; ix < x + w && ix < dungeon.Size.x; ix++) {
					yield return new Vector2Int(ix, iy);
				}
			}
		}
	}

	public interface Bounds {
		AppliedBounds Apply(AppliedBounds parent);
	}

	public class FullBounds : Bounds {
		public AppliedBounds Apply(AppliedBounds parent) {
			return parent;
		}
	}

	[System.Serializable]
	public class FixedBounds : Bounds {
		[HorizontalGroup(Width = 0.5f)]
		[LabelText("T")]
		[SerializeField] int left;
		[HorizontalGroup(Width = 0.5f)]
		[LabelText("L")]
		[SerializeField] int top;
		[HorizontalGroup("row2", Width = 0.5f)]
		[LabelText("W")]
		[SerializeField] int width;
		[HorizontalGroup("row2", Width = 0.5f)]
		[LabelText("H")]
		[SerializeField] int height;

		public FixedBounds(int left, int top, int width, int height) {
			this.left = left;
			this.top = top;
			this.width = width;
			this.height = height;
		}

		public AppliedBounds Apply(AppliedBounds parent) {
			return new AppliedBounds(parent.x + left, parent.y + top, width, height);
		}
	}

	[System.Serializable]
	public class InsetBounds : Bounds {
		[HorizontalGroup(Width = 0.5f)]
		[LabelText("L")]
		[SerializeField] int left;
		[HorizontalGroup(Width = 0.5f)]
		[LabelText("R")]
		[SerializeField] int right;
		[HorizontalGroup("row2", Width = 0.5f)]
		[LabelText("T")]
		[SerializeField] int top;
		[HorizontalGroup("row2", Width = 0.5f)]
		[LabelText("B")]
		[SerializeField] int bottom;

		public InsetBounds(int amt) : this(amt, amt, amt, amt) { }

		public InsetBounds(int leftAndRight, int topAndBottom) : this(leftAndRight, leftAndRight, topAndBottom, topAndBottom) { }

		public InsetBounds(int left, int right, int top, int bottom) {
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}

		public AppliedBounds Apply(AppliedBounds parent) {
			return new AppliedBounds(parent.x + left, parent.y + top, parent.w - right - left, parent.h - bottom - top);
		}
	}

	[System.Serializable]
	public class InsetPercentBounds : Bounds {
		[HorizontalGroup(Width = 0.5f)]
		[LabelText("L%")]
		[Range(0, 1)] [SerializeField] float percentLeft;
		[HorizontalGroup(Width = 0.5f)]
		[LabelText("R%")]
		[Range(0, 1)] [SerializeField] float percentRight;
		[HorizontalGroup("row2", Width = 0.5f)]
		[LabelText("T%")]
		[Range(0, 1)] [SerializeField] float percentTop;
		[HorizontalGroup("row2", Width = 0.5f)]
		[LabelText("B%")]
		[Range(0, 1)] [SerializeField] float percentBottom;

		public InsetPercentBounds(float amt) : this(amt, amt, amt, amt) { }

		public InsetPercentBounds(float leftAndRight, float topAndBottom) : this(leftAndRight, leftAndRight, topAndBottom, topAndBottom) { }

		public InsetPercentBounds(float percentLeft, float percentRight, float percentTop, float percentBottom) {
			this.percentLeft = percentLeft;
			this.percentRight = percentRight;
			this.percentTop = percentTop;
			this.percentBottom = percentBottom;
		}

		public AppliedBounds Apply(AppliedBounds parent) {
			int left = Mathf.FloorToInt(percentLeft * parent.w);
			int right = Mathf.FloorToInt(percentRight * parent.w);
			int top = Mathf.FloorToInt(percentTop * parent.h);
			int bottom = Mathf.FloorToInt(percentBottom * parent.h);
			return new AppliedBounds(parent.x + left, parent.y + top, parent.w - right - left, parent.h - bottom - top);
		}
	}

	[System.Serializable]
	public class CenteredBounds : Bounds {
		[Range(0, 1)] [SerializeField] float percentCX;
		[Range(0, 1)] [SerializeField] float percentCY;
		[Range(0, 1)] [SerializeField] float percentW;
		[Range(0, 1)] [SerializeField] float percentH;

		public CenteredBounds(float percentCX, float percentCY, float percentW, float percentH) {
			this.percentCX = percentCX;
			this.percentCY = percentCY;
			this.percentW = percentW;
			this.percentH = percentH;
		}

		public AppliedBounds Apply(AppliedBounds parent) {
			int halfW = Mathf.FloorToInt(parent.w * percentW / 2);
			int halfH = Mathf.FloorToInt(parent.h * percentH / 2);
			int cx = Mathf.FloorToInt(parent.w * percentCX);
			int cy = Mathf.FloorToInt(parent.h * percentCY);
			return new AppliedBounds(parent.x + cx - halfW, parent.y + cy - halfH, halfW * 2, halfH * 2);
		}
	}

	[System.Serializable]
	public class FixedCenterBounds : Bounds {
		[Range(0, 1)] [SerializeField] float percentCX;
		[Range(0, 1)] [SerializeField] float percentCY;
		[SerializeField] int width;
		[SerializeField] int height;

		public FixedCenterBounds(float percentCX, float percentCY, int width, int height) {
			this.percentCX = percentCX;
			this.percentCY = percentCY;
			this.width = width;
			this.height = height;
		}

		public AppliedBounds Apply(AppliedBounds parent) {
			int cx = Mathf.FloorToInt(parent.w * percentCX);
			int cy = Mathf.FloorToInt(parent.h * percentCY);
			return new AppliedBounds(parent.x + cx - (width / 2), parent.y + cy - (height / 2), width, height);
		}
	}

	/// <summary>
	/// Bounds specification that takes in a center point and int bounds, and just returns the applicable applied bounds.
	/// </summary>
	[System.Serializable]
	public class AbsoluteBounds : Bounds {
		[SerializeField] int cx;
		[SerializeField] int cy;
		[SerializeField] int width;
		[SerializeField] int height;

		public AbsoluteBounds(int cx, int cy, int width, int height) {
			this.cx = cx;
			this.cy = cy;
			this.width = width;
			this.height = height;
		}

		public AppliedBounds Apply(AppliedBounds parent) {
			return new AppliedBounds(parent.x + cx - (width / 2), parent.y + cy - (height / 2), width, height);
		}
	}
}

namespace DungeonestCrab.Dungeon.Generator {
	public class Stamper : IAlterer {
		private readonly ISource source;
		private readonly bool passTerrains;
		private readonly TerrainSO terrain;
		private readonly Bounds bounds;

		public Stamper(ISource source, TerrainSO terrain, bool passTerrains, Bounds bounds) {
			this.source = source;
			this.terrain = terrain;
			this.passTerrains = passTerrains;
			this.bounds = bounds;
		}

		public bool Modify(TheDungeon generator, IRandom rand) {
			AppliedBounds appliedBounds = bounds.Apply(generator.Bounds);
			Stamp stamp = new Stamp(generator, passTerrains, appliedBounds);

			source.Generate(stamp, rand);
			Debug.Log("Stamping " + source + ":\n" + TheDungeon.Visualize(stamp.Tiles));

			foreach (Vector2Int adjPoint in appliedBounds.AllTiles(generator)) {
				int y = appliedBounds.y, x = appliedBounds.x;
				TileSpec tile = generator.GetTileSpecSafe(adjPoint);
				int mx = adjPoint.x - x, my = adjPoint.y - y;
				Tile at = stamp.At(mx, my);
				if (at != Tile.Unset && !tile.Immutable) {
					tile.Tile = at;
					tile.Terrain = terrain;
					tile.Style = stamp.StyleAt(mx, my);
				}
			}
			return true;
		}

		public bool[,] DebugModify(int w, int h, IRandom rand) {
			Stamp stamp = new Stamp(w, h);
			source.Generate(stamp, rand);

			bool[,] tiles = new bool[w, h];

			for (int iy = 0; iy < h; iy++) {
				for (int ix = 0; ix < w; ix++) {
					if (stamp.Tiles[iy, ix] != Tile.Unset) {
						tiles[ix, iy] = true;
					}
				}
			}
			return tiles;
		}
	}
}