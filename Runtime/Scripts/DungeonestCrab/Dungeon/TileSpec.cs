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
		public readonly Vector2Int Coords;
		public Tile Tile;
		public TerrainSO Terrain;
		public int Style;
		public bool Immutable;
		public List<string> Flags = new List<string>();
		public List<Entity> Entities = new List<Entity>();

		private Adjacency _cachedTileAdjacencies;
		private Adjacency _cachedTerrainAdjacencies;

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
			get => Terrain.ShouldDrawAsFloor(Tile) || Style == (int)PaintStyle.SunkenFlooded;
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