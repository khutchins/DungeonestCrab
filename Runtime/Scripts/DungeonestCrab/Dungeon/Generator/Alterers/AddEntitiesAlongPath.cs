using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
	/// <summary>
	/// Places entities along an A-star path.
	/// </summary>
	public class AddEntitiesAlongPath : IEntityAdder {

		public delegate List<Vector2Int> PathComputer(TheDungeon generator, IRandom random);

		private readonly EntitySource _source;
		private readonly int _minRequired;
		private readonly bool _setAngles;
		private readonly float _additionalYRotation;
		private readonly Vector2Int _startingOffset;
		private readonly PathComputer _pathComputer;
		private readonly IMatcher _matcher;

		/// <summary>
		/// Alterer for adding entities along an A-star path. Will fail if no path is possible.
		/// </summary>
		/// <param name="source">The EntitySource to provide entities.</param>
		/// <param name="from">Point to start pathing from (entity will be placed here)</param>
		/// <param name="to">Point to path to</param>
		/// <param name="setAngles">If true, will set entity's y position to follow path (e.g. footsteps)</param>
		/// <param name="startingOffset">Offset from which the starting position will have come from. Useful with setAngles.</param>
		/// <param name="matcher">Criteria that must be met to place an entity on a tile (e.g. certain type of terrain). If null, will place anywhere.</param>
		/// <param name="minRequired">Minimum path length or else alterer will fail.</param>
		public AddEntitiesAlongPath(EntitySource source, PathComputer pathComputer, bool setAngles, float additionalYRotation = 0, Vector2Int startingOffset = new Vector2Int(), IMatcher matcher = null, int minRequired = -1) {
			_source = source;
			_minRequired = minRequired;
			_setAngles = setAngles;
			_startingOffset = startingOffset;
			_additionalYRotation = additionalYRotation;
			_pathComputer = pathComputer;
			_matcher = matcher ?? TileMatcher.MatchingAll();
		}

		public override bool Modify(TheDungeon generator, IRandom rand) {
			List<Vector2Int> path = _pathComputer.Invoke(generator, rand);

			// Path couldn't be generated.
			if (path == null) {
				return false;
			}

			if (path.Count > 0) {
				Vector2Int last = new Vector2Int(path[0].x + _startingOffset.x, path[0].y + _startingOffset.y);

				for (int i = 0; i < path.Count; i++) {
					Vector2Int coord = path[i];
					Vector2Int next = path.Count > i + 1 ? path[i + 1] : coord;
					Vector2 dir = new Vector2(next.x - last.x, last.y - next.y);
					float angle = _additionalYRotation + (_setAngles ? Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) : 0);

					TileSpec tile = generator.GetTileSpec(coord);
					if (_matcher.Matches(tile)) {
						generator.AddEntity(new Entity(coord, i, _source.GetPair(rand), null, angle));
					}
					last = coord;
				}
			}

			// Do this at the end, since I'll want to continue after this in the worst case scenario.
			return path.Count > _minRequired;
		}

		public class Builder {
			private EntitySource _source;
			private int _minRequired = -1;
			private bool _setAngles;
			private float _additionalYRotation;
			private Vector2Int _startingOffset;
			private PathComputer _pathComputer;
			private IMatcher _matcher;

			public Builder(EntitySource source, PathComputer computer) {
				_source = source;
				_pathComputer = computer;
			}

			public Builder SetMinRequired(int amount) {
				_minRequired = amount;
				return this;
			}

			public Builder SetMatcher(IMatcher matcher) {
				_matcher = matcher;
				return this;
			}

			public Builder SetAngles(bool setAngles, Vector2Int startingOffset = new Vector2Int()) {
				_setAngles = setAngles;
				_startingOffset = startingOffset;
				return this;
			}

			public Builder SetAdditionalYRotation(float additionalYRotation) {
				_additionalYRotation = additionalYRotation;
				return this;
			}

			public static Builder ForShortestPath(EntitySource source, Vector2Int from, Vector2Int to, TerrainSO requiredTerrain = null) {
				Builder builder = new Builder(source, (TheDungeon generator, IRandom rand) => {
					DungeonPather pather = new DungeonPather(generator, DungeonPather.UniformCostTerrainSpecificWalkablePather(requiredTerrain));
					IEnumerable<Vector2Int> path = pather.FindPath(from, to);
					if (path == null) {
						Debug.LogWarningFormat("No path existed from {0} to {1}!", from, to);
						return null;
					}

					List<Vector2Int> pathList = path.ToList();
					pathList.Insert(0, from);
					return pathList;
				});
				return builder;
			}

			public static Builder ToFarthestPoint(EntitySource source, Vector2Int from, TerrainSO requiredTerrain = null) {
				Builder builder = new Builder(source, (TheDungeon generator, IRandom rand) => {
					DungeonPather pather = new DungeonPather(generator, DungeonPather.UniformCostTerrainSpecificWalkablePather(requiredTerrain));
					IEnumerable<Vector2Int> path = pather.FindPathToFarthestPoint(from);

					List<Vector2Int> pathList = path.ToList();
					pathList.Insert(0, from);

					return pathList;
				});
				return builder;
			}

			public AddEntitiesAlongPath Build() {
				return new AddEntitiesAlongPath(_source, _pathComputer, _setAngles, _additionalYRotation, _startingOffset, _matcher, _minRequired);
			}
		}
	}
}