using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
	/// <summary>
	/// Generation source that fills all empty space with a maze.
	/// 
	/// Only guarantees connection if placed in a contiguous space.
	/// </summary>
	[System.Serializable]
	public class SourceMaze : ISource {
		[Range(0f, 1f)]
		[Tooltip("Bias towards straight lines in the maze.")]
		[SerializeField] float _straightBias;
		[Range(0f, 1f)]
		[Tooltip("Percent of dead ends that should be turned into loops.")]
		[SerializeField] float _braidPercent;
		[Tooltip("Whether the maze should conditionally increase the inset to avoid not having a gap between a maze and its surroundings.")]
		[SerializeField] bool _conservative;

		/// <summary>
		/// Creates a maze source.
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="straightBias">Bias towards straight lines in the maze.</param>
		/// <param name="conservative">Whether the maze should attempt to avoid accidentally exposing its innards to an existing floor area.</param>
		/// <param name="braidPercent">Percent of dead ends that should be turned into loops.</param>
		public SourceMaze(Tile tile = Tile.Floor, float straightBias = 0.5F, bool conservative = false, float braidPercent = 0f) : base(tile) {
			_straightBias = straightBias;
			_conservative = conservative;
			_braidPercent = braidPercent;
		}

		public override void Generate(Stamp stamp, IRandom rand) {
			for (int y = 0; y < stamp.H; y += 2) {
				for (int x = 0; x < stamp.W; x += 2) {
					if (!IsSettableAt(stamp, new DirectedPoint(x, y))) continue;
					GrowMaze(stamp, x, y, rand);
				}
			}
		}

		private void GrowMaze(Stamp stamp, int sx, int sy, IRandom rand) {
			List<DirectedPoint> cells = new List<DirectedPoint>();

			stamp.MaybeSetAt(sx, sy, _tileToSet);
			cells.Add(new DirectedPoint(sx, sy, rand.FromEnum<Dir>()));
			Dir lastDir = cells[0].dir;
			while (cells.Count > 0) {
				DirectedPoint cell = cells[cells.Count - 1];

				List<DirectedPoint> adjs = cell.adjacencies2Away().Where(
					pt => IsSettableAt(stamp, pt)
				).ToList();

				if (adjs.Count > 0) {
					DirectedPoint next;
					if (rand.WithPercentChance(this._straightBias) && adjs.Where(x => x.dir == lastDir).Count() > 0) {
						next = adjs.Where(x => x.dir == lastDir).First();
					} else {
						next = rand.From(adjs);
					}
					DirectedPoint between = DirectedPoint.between(cell, next);
					stamp.MaybeSetAt(next.x, next.y, _tileToSet);
					stamp.MaybeSetAt(between.x, between.y, _tileToSet);
					lastDir = next.dir;

					cells.Add(next);
				} else {
					cells.Remove(cell);
				}
			}

			if (_braidPercent > 0f) {
				List<Vector2Int> deadEnds = GetDeadEnds(stamp);
				int totalDeadEnds = deadEnds.Count;
				int targetBraids = Mathf.FloorToInt(totalDeadEnds * _braidPercent);

				int set = 0;

				while (deadEnds.Count > 0 && set < targetBraids) {
					int idx = rand.Next(deadEnds.Count);
					Vector2Int c = deadEnds[idx];

					// It's possible that joining a dead end will clear two of them.
					// If so, recompute the target and move on.
					if (!IsDeadEnd(stamp, c)) {
						totalDeadEnds--;
						targetBraids = Mathf.FloorToInt(totalDeadEnds * _braidPercent);
						deadEnds.RemoveAt(idx);
						continue;
					}

					List<Vector2Int> validNeighbors = new List<Vector2Int>();
					if (stamp.CanSetAt(c.x - 1, c.y) && stamp.CanSetAt(c.x - 2, c.y) && stamp.At(c.x - 1, c.y) == Tile.Unset) validNeighbors.Add(new Vector2Int(c.x - 1, c.y));
					if (stamp.CanSetAt(c.x + 1, c.y) && stamp.CanSetAt(c.x + 2, c.y) && stamp.At(c.x + 1, c.y) == Tile.Unset) validNeighbors.Add(new Vector2Int(c.x + 1, c.y));
					if (stamp.CanSetAt(c.x, c.y - 1) && stamp.CanSetAt(c.x, c.y - 2) && stamp.At(c.x, c.y - 1) == Tile.Unset) validNeighbors.Add(new Vector2Int(c.x, c.y - 1));
					if (stamp.CanSetAt(c.x, c.y + 1) && stamp.CanSetAt(c.x, c.y + 2) && stamp.At(c.x, c.y + 1) == Tile.Unset) validNeighbors.Add(new Vector2Int(c.x, c.y + 1));

					// No valid neighbors to set. It can happen if stamping on an
					// existing terrain. If so, recompute the target and move on.
					if (validNeighbors.Count < 1) {
						totalDeadEnds--;
						targetBraids = Mathf.FloorToInt(totalDeadEnds * _braidPercent);
						deadEnds.RemoveAt(idx);
						continue;
					}

					Vector2Int neighborToRemove = rand.From(validNeighbors);
					stamp.MaybeSetAt(neighborToRemove, _tileToSet);
					deadEnds.RemoveAt(idx);
					set++;
				}
			}
		}

		private bool IsSettableAt(Stamp stamp, DirectedPoint pt) {
			if (!_conservative) return stamp.CanSetAt(pt.x, pt.y) && stamp.At(pt.x, pt.y) == Tile.Unset;
			return stamp.CanSetAt(pt.x, pt.y) && stamp.At(pt.x, pt.y) == Tile.Unset
				&& stamp.CanSetAt(pt.x + 1, pt.y)
				&& stamp.CanSetAt(pt.x - 1, pt.y)
				&& stamp.CanSetAt(pt.x, pt.y + 1)
				&& stamp.CanSetAt(pt.x, pt.y - 1);
		}

		private bool IsDeadEnd(Stamp stamp, Vector2Int pt) {
			if (stamp.At(pt) == Tile.Unset) return false;

			int realConnections = 0;
			foreach (Vector2Int adj in pt.Adjacencies1Away()) {
				if (stamp.CanSetAt(adj) && stamp.At(adj) != Tile.Unset) realConnections++;
				if (realConnections > 1) return false;
			}
			return realConnections == 1;
		}

		private List<Vector2Int> GetDeadEnds(Stamp stamp) {
			List<Vector2Int> deadEnds = new List<Vector2Int>();

			for (int y = 0; y < stamp.H; y += 2) {
				for (int x = 0; x < stamp.W; x += 2) {
					Vector2Int pt = new Vector2Int(x, y);
					if (IsDeadEnd(stamp, pt)) {
						deadEnds.Add(pt);
					}
				}
			}

			return deadEnds;
		}
	}
}
