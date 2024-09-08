using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon {
	public enum Tile {
		Unset = -1,
		Wall = 0,
		Floor = 1,
	}

	public class TileSpec : IEquatable<TileSpec> {
		public const string DRAW_STYLE_FLOOR = "drawStyle:floor";
		public const string DRAW_STYLE_WALL = "drawStyle:wall";
		public const string DRAW_STYLE_ALL = "drawStyle:all";
		public const string DRAW_STYLE_NONE = "drawStyle:none";
		public const string STYLE_PREFIX = "style";

		public readonly Vector2Int Coords;
		public Tile Tile;
		public TerrainSO Terrain;
		public int Style;
		public bool Immutable;
		private HashSet<string> _tags = new HashSet<string>();
		public List<Entity> Entities = new List<Entity>();

		private Adjacency _cachedTileAdjacencies;
		private Adjacency _cachedTerrainAdjacencies;
		private DrawStyle _drawStyle = DrawStyle.NoOverride;

		[Flags]
		enum DrawStyle {
			None = -1,
			NoOverride = 0,
			Wall = 1 << 0,
			Floor = 1 << 1,
		}

		[Flags]
		// These are intentionally ordered to allow indexing based on
		// neighboring coordinates, which yeah, isn't robust, but it
		// does simplify things.
		public enum Adjacency {
			SW = 1 << 0,
			S = 1 << 1,
			SE = 1 << 2,
			W = 1 << 3,
			Here = 1 << 4,
			E = 1 << 5,
			NW = 1 << 6,
			N = 1 << 7,
			NE = 1 << 8,
		}

		public TileSpec(Vector2Int coords, Tile tile, TerrainSO terrain, int style, bool immutable) {
			Coords = coords;
			Tile = tile;
			Terrain = terrain;
			Style = style;
			Immutable = immutable;
		}

		public void AddEntity(Entity entity) {
			Entities.Add(entity);
		}

		public void AddTag(string tag) {
			if (_tags == null) _tags = new HashSet<string>();
			_tags.Add(tag);

			if (tag == DRAW_STYLE_ALL) {
				_drawStyle = DrawStyle.Wall | DrawStyle.Floor;
			} else if (tag == DRAW_STYLE_FLOOR) {
				_drawStyle = DrawStyle.Floor;
			} else if (tag == DRAW_STYLE_WALL) {
				_drawStyle = DrawStyle.Wall;
			} else if (tag == DRAW_STYLE_NONE) {
				_drawStyle = DrawStyle.None;
			}
		}

		public bool HasTag(string tag) {
			return _tags != null && _tags.Contains(tag);
		}

		public string GetTagType(string type) {
			foreach (string tag in _tags) {
				var split = tag.Split(':', 2);
				if (split[0] == type) {
					if (split.Length == 1) return "";
					else return split[1];
				}
			}
			return null;
		}

		public bool EntityBlocksMovement() {
			foreach (Entity entity in Entities) {
				if (entity.Type.BlocksMovement) return true;
			}
			return false;
		}

		public bool Walkable {
			get => Tile == Tile.Floor && !EntityBlocksMovement();
		}

		public bool DrawAsFloor {
			get => _drawStyle != DrawStyle.None 
				&& ((Terrain.ShouldDrawAsFloor(Tile) && _drawStyle == DrawStyle.NoOverride) 
					|| (_drawStyle & DrawStyle.Floor) != 0);
		}

		public bool DrawAdjacentWalls {
			get => DrawAsFloor || _drawStyle == DrawStyle.None;
		}

		public bool DrawWalls {
			get => _drawStyle != DrawStyle.None
				&& ((!Terrain.ShouldDrawAsFloor(Tile) && _drawStyle == DrawStyle.NoOverride) 
					|| (_drawStyle & DrawStyle.Wall) != 0);
		}

		public float CeilingOffset {
			get => Terrain.CeilingOffset;
		}

		public bool HasCeiling {
			get => Terrain.HasCeiling;
		}

		public float GroundOffset {
			get => Mathf.Max(Terrain.GroundOffset, Style == (int)PaintStyle.SunkenFlooded ? 0.2F : 0);
		}

		public float TileCarvingCost {
			get {
				if (Immutable) return -1;
				if (Terrain == null) return Tile == Tile.Wall ? 6 : 1;
				return Terrain.TileCarvingCost(Tile);
			}
		}

		public bool IsDrawable {
			get => Terrain != null && Tile != Tile.Unset;
		}

		public void SetTerrainIfNull(TerrainSO terrain) {
			if (Terrain == null) Terrain = terrain;
		}

		public override bool Equals(object obj) {
			if (obj == null || !this.GetType().Equals(obj.GetType())) return false;
			return Equals((TileSpec)obj);
		}

		public bool Equals(TileSpec other) {
			return other != null &&
				   EqualityComparer<Vector2Int>.Default.Equals(Coords, other.Coords) &&
				   Tile == other.Tile &&
				   EqualityComparer<TerrainSO>.Default.Equals(Terrain, other.Terrain) &&
				   Style == other.Style &&
				   Immutable == other.Immutable &&
				   EqualityComparer<List<Entity>>.Default.Equals(Entities, other.Entities);
		}

		public bool AreTerrainsTheSameInDirections(Adjacency directions) {
			return (_cachedTerrainAdjacencies & directions) == directions;
		}

		public bool AreTileTypesTheSameInDirections(Adjacency directions) {
			return (_cachedTileAdjacencies & directions) == directions;
		}

		public void SetTileAdjacencies(Adjacency adjacencies) {
			_cachedTileAdjacencies = adjacencies;
		}

		public void SetTerrainAdjacencies(Adjacency adjacencies) {
			_cachedTerrainAdjacencies = adjacencies;
		}

		public override int GetHashCode() {
			return HashCode.Combine(Coords, Tile, Terrain, Style, Immutable, Entities);
		}
	}
}