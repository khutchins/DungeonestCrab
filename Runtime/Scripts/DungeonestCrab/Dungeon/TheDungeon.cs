using Pomerandomian;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DungeonestCrab.Dungeon {
	public enum Trait {
		None = -1,
		// All floors are covered by water
		Flooded = 0,
		// No walls above zero, all unset terrain is considered pit.
		CeilinglessPit = 1,
		// As long as terrain has been set, will use wall terrain's wall texture
		// instead of adjacent floor's.
		InvasiveWalls = 3,
		// No ceiling. Walls still set if applicable
		Ceilingless = 6,
	}

	public class TheDungeon {
		private TileSpec[,] _tiles;
		public HashSet<TileSpec> TilesWithEntities;
		public List<Entity> Entities;
		public readonly AppliedBounds Bounds;
		public readonly Vector2Int Size;
		public Trait Trait;
		public float FogDensity = 0.01f;
		public Color FogColor = Color.white;

		public readonly IRandom ConsistentRNG;
		/// <summary>
		/// A RNG that will not have consistent results each time. 
		/// Only use this on things that it makes sense to change,
		/// like a character that should change places every time
		/// you come back to a scene. If using this RNG to choose
		/// a range of events, NEVER use the consistent RNG for 
		/// those events, as it will result in inconsistent results.
		/// </summary>
		public readonly IRandom NonConsistentRNG;

		private float[,] _pathMap;
		private int[,] _cachedRegionMap;
		private int _cachedMaxRegion = -1;

		public TheDungeon(int w, int h, IRandom consistentRNG = null, IRandom inconsistentRNG = null) {
			Size = new Vector2Int(w, h);
			Bounds = new AppliedBounds(0, 0, w, h);
			TilesWithEntities = new HashSet<TileSpec>();
			Entities = new List<Entity>();
			_tiles = CreateTiles();
			_pathMap = new float[h, w];
			this.ConsistentRNG = consistentRNG ?? new SystemRandom();
			this.NonConsistentRNG = inconsistentRNG ?? new SystemRandom();
		}

		private TileSpec[,] CreateTiles() {
			TileSpec[,] tiles = new TileSpec[Size.y, Size.x];
			for (int y = 0; y < Size.y; y++) {
				for (int x = 0; x < Size.x; x++) {
					tiles[y, x] = new TileSpec(new Vector2Int(x, y), Tile.Wall, null, (int)PaintStyle.Default, x == 0 || x == Size.x - 1 || y == 0 || y == Size.y - 1);
				}
			}
			return tiles;
		}

		public bool Contains(int x, int y) {
			return Size.InBounds(x, y);
		}

		public bool Contains(Vector2Int pt) {
			return Size.InBounds(pt);
		}

		public TileSpec GetTileSpec(int x, int y) {
			return _tiles[y, x];
		}

		public TileSpec GetTileSpec(Vector2Int pt) {
			return _tiles[pt.y, pt.x];
		}

		public TileSpec GetTileSpecSafe(int x, int y) {
			if (!Contains(x, y)) return null;
			return _tiles[y, x];
		}

		public TileSpec GetTileSpecSafe(Vector2Int coords) {
			if (!Size.InBounds(coords)) return null;
			return _tiles[coords.y, coords.x];
		}

		public Tile TryGetTile(int x, int y, Tile defaultVal) {
			if (!Contains(x, y)) return defaultVal;
			return _tiles[y, x].Tile;
		}

		public Tile TryGetTile(Vector2Int pt, Tile defaultVal) {
			return TryGetTile(pt.x, pt.y, defaultVal);
		}

		public Tile GetTile(int x, int y) {
			return _tiles[y, x].Tile;
		}

		public Tile GetTile(Vector2Int pt) {
			return _tiles[pt.y, pt.x].Tile;
		}

		public IEnumerable<TileSpec> AllTiles() {
			for (int y = 0; y < Size.y; y++) {
				for (int x = 0; x < Size.x; x++) {
					yield return _tiles[y, x];
				}
			}
		}

		public void AddEntity(Entity entity) {
			TileSpec tile = GetTileSpec(entity.Tile);
			tile.AddEntity(entity);
			TilesWithEntities.Add(tile);
			Entities.Add(entity);
			if (entity.Type.BlocksMovement) {
				_pathMap[entity.Tile.y, entity.Tile.x] = -1;
			}
		}

		public void AddJunction(Vector2Int pt, TerrainSO terrain) {
			TileSpec tile = GetTileSpec(pt);
			tile.Tile = Tile.Floor;
			tile.SetTerrainIfNull(terrain);
			// TODO: Add doors here if we want to
			//addEntity(new Entity(pt, Entity.Type.Door));
		}

		public Entity FindEntity(EntitySO type, string id = null) {
			return Entities.FirstOrDefault(x => x.Type == type.Entity && (id == null || id == "" || x.EntityID == id));
		}

		public Entity FindEntity(EntitySpec type, string id = null) {
			return Entities.FirstOrDefault(x => x.Type == type && (id == null || id == "" || x.EntityID == id));
		}

		public float TileCarvingCost(TileSpec spec) {
			return spec.TileCarvingCost;
		}


		/// <summary>
		/// Returns the number of regions.
		/// NOTE: This is as expensive as a call to computeRegions.
		/// </summary>
		/// <returns></returns>
		public int ComputeRegionCount() {
			ComputeRegions(out int res);
			return res;
		}

		public int[,] ComputeRegions(out int maxRegion) {
			if (_cachedRegionMap != null) {
				maxRegion = _cachedMaxRegion;
				return _cachedRegionMap;
			}
			maxRegion = 0;
			int[,] regions = new int[Size.y, Size.x];

			for (int y = 0; y < Size.y; y++) {
				for (int x = 0; x < Size.x; x++) {
					regions[y, x] = -1;
				}
			}

			foreach (TileSpec tile in AllTiles()) {
				int y = tile.Coords.y, x = tile.Coords.x;
				// Walls aren't in regions.
				if (regions[y, x] != -1 || !tile.Walkable) continue;

				regions[y, x] = maxRegion;
				List<Vector2Int> unvisitedPoints = new List<Vector2Int> { tile.Coords };

				while (unvisitedPoints.Count > 0) {
					Vector2Int head = unvisitedPoints[0];
					IEnumerable<TileSpec> adjs = TileAdjacencies(head);
					foreach (TileSpec adj in adjs) {
						if (!Contains(adj.Coords)) continue;
						if (regions[adj.Coords.y, adj.Coords.x] != -1) continue;
						if (!adj.Walkable) continue;
						regions[adj.Coords.y, adj.Coords.x] = maxRegion;
						unvisitedPoints.Add(adj.Coords);
					}
					unvisitedPoints.RemoveAt(0);
				}

				maxRegion++;
			}

			// Max region is incremented one time too many
			// and will report the incorrect value unless
			// we decrement it.
			maxRegion--;
			Debug.Log(VisualizeRegions(regions));
			_cachedMaxRegion = maxRegion;
			_cachedRegionMap = regions;
			return regions;
		}

		/// <summary>
		/// Regenerates all the cached information, like tile adjacencies.
		/// Might be expensive, so try to only do it when necessary.
		/// </summary>
		public void UpdateDungeonComputations() {
			PopulateTileCaches();
		}

		private void PopulateTileCaches() {
			foreach (TileSpec tile in AllTiles()) {
				PopulateTileCache(tile);
			}
		}

		private void PopulateTileCache(TileSpec tile) {
			TileSpec.Adjacency tileAdjs = TileSpec.Adjacency.Here;
			TileSpec.Adjacency terrainAdjs = TileSpec.Adjacency.Here;
			for (int my = -1; my <= 1; my++) {
				for (int mx = -1; mx <= 1; mx++) {

					TileSpec neighbor = GetTileSpecSafe(tile.Coords.x + mx, tile.Coords.y + my);
					TileSpec.Adjacency mod = (TileSpec.Adjacency)(1 << ((my + 1) * 3 + (mx + 1)));
					// Hacky way to index into adjacencies. Probably not a good idea.
					if (neighbor != null) {
						if (tile.Tile == neighbor.Tile) tileAdjs |= mod;
						if (tile.Terrain == neighbor.Terrain) terrainAdjs |= mod;
					}
				}
			}
			tile.SetTileAdjacencies(tileAdjs);
			tile.SetTerrainAdjacencies(terrainAdjs);
		}

		// Returns list of all adjacent tiles.
		public IEnumerable<TileSpec> TileAdjacencies(Vector2Int pt) {
			foreach (Vector2Int adj in pt.Adjacencies1Away()) {
				TileSpec tile = GetTileSpecSafe(adj);
				if (tile != null) yield return tile;
			}
		}

		// Returns walkable adjacency list for given point. Starts
		// at top left and rotates clockwise around the point. Used
		// to compute whether taking this square would block movement.
		public bool[] AdjacentWalkableList(Vector2Int pt) {
			return new bool[] {
				WalkableAt(pt.x - 1, pt.y - 1),
				WalkableAt(pt.x, pt.y - 1),
				WalkableAt(pt.x + 1, pt.y - 1),
				WalkableAt(pt.x + 1, pt.y),
				WalkableAt(pt.x + 1, pt.y + 1),
				WalkableAt(pt.x, pt.y + 1),
				WalkableAt(pt.x - 1, pt.y + 1),
				WalkableAt(pt.x - 1, pt.y),
			};
		}

		public bool WalkableAt(int x, int y) {
			TileSpec tileSpec = GetTileSpecSafe(x, y);
			if (tileSpec == null) return false;
			return tileSpec.Walkable;
		}

		public bool WalkableAt(Vector2Int coords) {
			if (!Size.InBounds(coords)) return false;
			TileSpec tileSpec = GetTileSpecSafe(coords);
			if (tileSpec == null) return false;
			return tileSpec.Walkable;
		}

		public bool WalkableAt(TileSpec tile, Vector2Int offset) {
			if (tile == null) return false;
			return WalkableAt(tile.Coords + offset);
		}

		public static float RotationForDir(string dir) {
			return dir switch {
				"E" => 90,
				"S" => 180,
				"W" => 270,
				_ => 0,
			};
		}

		public string Visualize() {
			StringBuilder str = new StringBuilder();
			for (int y = Size.y - 1; y >= 0; y--) {
				for (int x = 0; x < Size.x; x++) {
					str.Append(TerrainSO.GenericCharForTile(GetTile(x, y)));
				}
				str.Append("\n");
			}
			return str.ToString();
		}

		public static string Visualize(Tile[,] tiles) {
			StringBuilder str = new StringBuilder();
			int h = tiles.GetLength(0);
			int w = tiles.GetLength(1);
			for (int y = h - 1; y >= 0; y--) {
				for (int x = 0; x < w; x++) {
					str.Append(TerrainSO.GenericCharForTile(tiles[y, x]));
				}
				str.Append("\n");
			}
			return str.ToString();
		}

		private string VisualizeRegions(int[,] regions) {
			StringBuilder str = new StringBuilder();
			for (int y = Size.y - 1; y >= 0; y--) {
				for (int x = 0; x < Size.x; x++) {
					str.Append(string.Format("{0,3}", regions[y, x]));
				}
				str.Append("\n");
			}
			return str.ToString();
		}
	}
}