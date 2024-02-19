using System.Collections.Generic;
using System.Linq;
using DungeonestCrab.Dungeon;
using Pomerandomian;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
	public class SourceCave : ISource {
		private readonly float _initialTileOdds;
		private readonly float _iterations;
		private readonly int _requiredNeighbors;
		private readonly int _maxNeighbors;
		private readonly bool _invert;

		/// <summary>
		/// A source that uses cellular automata to generate terrain.
		/// </summary>
		/// <param name="tileToSet">Tile to be set.</param>
		/// <param name="initialTileOdds">Initial chance for a tile to be set.</param>
		/// <param name="minNeighbors">Minimum number of neighbors for a tile to be "alive" next iteration.</param>
		/// <param name="iterations">How many iterations to simulate for.</param>
		/// <param name="invert">If true, set tiles will be considered dead, and unset live.</param>
		public SourceCave(Tile tileToSet, float initialTileOdds, int minNeighbors, bool invert = false, int iterations = 3) 
			: this(tileToSet, initialTileOdds, minNeighbors, 8, invert, iterations) {
		}

		/// <summary>
		/// A source that uses cellular automata to generate terrain. A max neighbors value other than eight will give strange results. 
		/// Either try it out first with StampViz, or just use the constructor without max neighbors.
		/// </summary>
		/// <param name="tileToSet">Tile to be set.</param>
		/// <param name="initialTileOdds">Initial chance for a tile to be set.</param>
		/// <param name="minNeighbors">Minimum number of neighbors for a tile to be "alive" next iteration.</param>
		/// <param name="maxNeighbors">Maximum Number of neighbors for a tile to be "alive" next iteration.</param>
		/// <param name="iterations">How many iterations to simulate for.</param>
		/// <param name="invert">If true, set tiles will be considered dead, and unset live.</param>
		public SourceCave(Tile tileToSet, float initialTileOdds, int minNeighbors, int maxNeighbors, bool invert = false, int iterations = 3) : base(tileToSet) {
			_initialTileOdds = initialTileOdds;
			_requiredNeighbors = minNeighbors;
			_maxNeighbors = maxNeighbors;
			_iterations = iterations;
			_invert = invert;
		}

		public override void Generate(Stamp stamp, IRandom rand) {
			int w = stamp.W;
			int h = stamp.H;

			for (int iy = 1; iy < h - 1; iy++) {
				for (int ix = 1; ix < w - 1; ix++) {
					if (stamp.At(ix, iy) == Tile.Unset && rand.WithPercentChance(_initialTileOdds)) {
						stamp.MaybeSetAt(ix, iy, _tileToSet);
					}
				}
			}

			Tile liveTile = _invert ? Tile.Unset : _tileToSet;
			Tile deadTile = _invert ? _tileToSet : Tile.Unset;

			for (int i = 0; i < _iterations; i++) {

				// List of points to turn into rooms
				List<Vector2Int> deadPoints = new List<Vector2Int>();
				// List of points to turn into walls
				List<Vector2Int> livePoints = new List<Vector2Int>();
				for (int iy = 1; iy < h - 1; iy++) {
					for (int ix = 1; ix < w - 1; ix++) {
						Vector2Int pt = new Vector2Int(ix, iy);
						Tile type = stamp.At(ix, iy);
						int wallNeighbors = _invert ? pt.AdjacenciesWithDiag().Where(pt2 => stamp.At(pt2.x, pt2.y) == Tile.Unset).Count() 
							: pt.AdjacenciesWithDiag().Where(pt2 => stamp.At(pt2.x, pt2.y) != Tile.Unset).Count();
						if (_requiredNeighbors <= wallNeighbors && wallNeighbors <= _maxNeighbors) livePoints.Add(pt);
						else deadPoints.Add(pt);
					}
				}


				livePoints.ForEach(pt => stamp.MaybeSetAt(pt.x, pt.y, liveTile));
				deadPoints.ForEach(pt => stamp.MaybeSetAt(pt.x, pt.y, deadTile));
			}
		}
	}
}