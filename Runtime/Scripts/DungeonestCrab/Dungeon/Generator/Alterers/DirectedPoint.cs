using System.Collections.Generic;
using System.Linq;

namespace DungeonestCrab.Dungeon {
	public enum Dir {
		Left = 0,
		Right = 1,
		Up = 2,
		Down = 3
	}

	public class DirectedPoint {
		public readonly int x;
		public readonly int y;
		public readonly Dir dir;

		public DirectedPoint(int x, int y) {
			this.x = x;
			this.y = y;
			this.dir = Dir.Down;
		}

		public DirectedPoint(int x, int y, Dir dir) {
			this.x = x;
			this.y = y;
			this.dir = dir;
		}

		public IEnumerable<DirectedPoint> adjacenciesWithDiag() {
			return new DirectedPoint[] {
				new DirectedPoint(x - 1, y - 1, Dir.Left),
				new DirectedPoint(x - 1, y, Dir.Left),
				new DirectedPoint(x - 1, y + 1, Dir.Left),
				new DirectedPoint(x, y - 1, Dir.Up),
				new DirectedPoint(x, y + 1, Dir.Down),
				new DirectedPoint(x + 1, y - 1, Dir.Right),
				new DirectedPoint(x + 1, y, Dir.Right),
				new DirectedPoint(x + 1, y + 1, Dir.Right),
			};
		}

		public List<DirectedPoint> adjacencies1Away() {
			return new DirectedPoint[] {
				new DirectedPoint(x - 1, y, Dir.Left),
				new DirectedPoint(x + 1, y, Dir.Right),
				new DirectedPoint(x, y - 1, Dir.Up),
				new DirectedPoint(x, y + 1, Dir.Down)
			}.ToList();
		}

		public List<DirectedPoint> adjacencies2Away() {
			return new DirectedPoint[] {
				new DirectedPoint(x - 2, y, Dir.Left),
				new DirectedPoint(x + 2, y, Dir.Right),
				new DirectedPoint(x, y - 2, Dir.Up),
				new DirectedPoint(x, y + 2, Dir.Down)
			}.ToList();
		}

		public override bool Equals(object obj) {
			if (obj == null || this.GetType() != obj.GetType()) {
				return false;
			}
			return this.x == ((DirectedPoint)obj).x && this.y == ((DirectedPoint)obj).y;
		}

		public override int GetHashCode() {
			return x.GetHashCode() ^ y.GetHashCode();
		}

		public override string ToString() {
			return x + " " + y + " " + dir;
		}

		public static DirectedPoint between(DirectedPoint p1, DirectedPoint p2) {
			return new DirectedPoint((p1.x + p2.x) / 2, (p1.y + p2.y) / 2, p2.dir);
		}

		public DirectedPoint InCurrentDir() {
			return InDir(dir);
		}

		public DirectedPoint InDir(Dir dir) {
			switch (dir) {
				case Dir.Left:
					return new DirectedPoint(x - 1, y, Dir.Left);
				case Dir.Right:
					return new DirectedPoint(x + 1, y, Dir.Right);
				case Dir.Up:
					return new DirectedPoint(x, y - 1, Dir.Up);
				case Dir.Down:
					return new DirectedPoint(x, y + 1, Dir.Down);
			}
			// Should not happen
			return new DirectedPoint(x, y + 1, Dir.Down);
		}

		public bool IsVertical() {
			return dir == Dir.Down || dir == Dir.Up;
		}

		public bool IsHorizontal() {
			return dir == Dir.Left || dir == Dir.Right;
		}
	}
}