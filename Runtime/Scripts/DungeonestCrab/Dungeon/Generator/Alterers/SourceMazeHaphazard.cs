using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
	[System.Serializable]
	public class SourceMazeHaphazard : ISource {
		[SerializeField] int _maxMoveDistance = 3;
		[SerializeField] int _moveMultiplier = 1;
		[SerializeField] float _insertionAttemptsPerTile = 2f;

		Vector2Int _currentPosition;

		/// <summary>
		/// A more eclectic maze generation algorithm.
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="maxMoveDistance"></param>
		/// <param name="insertionAttempts"></param>
		public SourceMazeHaphazard(Tile tile = Tile.Floor, int maxMoveDistance = 3, float insertionAttemptsPerTile = 2f, int moveMultiplier = 1) : base(tile) {
			_maxMoveDistance = maxMoveDistance;
			_insertionAttemptsPerTile = insertionAttemptsPerTile;
			_moveMultiplier = moveMultiplier;
		}

		public override void Generate(Stamp stamp, IRandom rand) {
			_moveMultiplier = Mathf.Max(_moveMultiplier, 1);
			_currentPosition = new Vector2Int(rand.Next(stamp.W), rand.Next(stamp.H));
			int attempts = Mathf.FloorToInt(stamp.W * stamp.H * _insertionAttemptsPerTile);
			for (int i = 0; i < attempts; i++) {
				GrowInDirection(stamp, (rand.Next(Mathf.FloorToInt(_maxMoveDistance/_moveMultiplier)) + 1) * _moveMultiplier, rand.FromEnum<Dir>());
			}
		}

		private void GrowInDirection(Stamp stamp, int amount, Dir dir) {
			for (int i = 0; i < amount; i++) {
				Vector2Int newPoint = PointInDirection(_currentPosition, dir);
				if (!Growable(stamp, newPoint, dir)) return;
				_currentPosition = newPoint;
				stamp.MaybeSetAt(_currentPosition, _tileToSet);
			}
		}

		private bool Growable(Stamp stamp, Vector2Int source, Dir dir) {
			if (!stamp.InBounds(source)) return false;
			if (stamp.At(source) == Tile.Floor) return true;
			if (stamp.At(PointInDirection(source, Perpendicular1(dir)), Tile.Wall) == Tile.Floor) return false;
			if (stamp.At(PointInDirection(source, Perpendicular2(dir)), Tile.Wall) == Tile.Floor) return false;
			return true;
		}

		private Dir Perpendicular1(Dir dir) {
            return dir switch {
                Dir.Up or Dir.Down => Dir.Left,
                _ => Dir.Up,
            };
        }

		private Dir Perpendicular2(Dir dir) {
			return dir switch {
				Dir.Up or Dir.Down => Dir.Right,
				_ => Dir.Down,
			};
		}

		private Vector2Int PointInDirection(Vector2Int source, Dir dir) {
            return dir switch {
                Dir.Left => source - new Vector2Int(1, 0),
                Dir.Right => source + new Vector2Int(1, 0),
                Dir.Up => source - new Vector2Int(0, 1),
                _ => source + new Vector2Int(0, 1),
            };
        }
	}
}