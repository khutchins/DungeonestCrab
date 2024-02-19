using System;
using System.Linq;
using UnityEngine;
using Pomerandomian;
using KH.Audio;
using KH.Tools;

namespace DungeonestCrab.Dungeon.Printer {
	[DefaultExecutionOrder(-1)]
	public class DungeonPrinter : MonoBehaviour {

		[SerializeField] DungeonReference GeneratorRef;
		public GameObject MovementBlocker;
		public bool MergeMeshes = true;

		public static DungeonPrinter Shared;

		private TheDungeon _dungeon;

		Transform EnvironmentHolder;
		Transform CollisionHolder;
		Transform StaticEntityHolder;
		Transform EntityHolder;
		private Transform _testHolder;

		protected void Awake() {
			Shared = this;
			MakeHolders(this.transform);
		}

		public void Print(TheDungeon dg) {
			foreach (TileSpec tileSpec in dg.AllTiles()) {
				int x = tileSpec.Coords.x, y = tileSpec.Coords.y;

				Entity floorReplacingEntity = tileSpec.Entities.FirstOrDefault(t => t.Type.ReplacesFloor);
				Entity ceilingReplacingEntity = tileSpec.Entities.FirstOrDefault(t => t.Type.ReplacesCeiling);

				GameObject blocker = null;
				bool walkable = tileSpec.Walkable;
				if (!walkable) {
					blocker = Instantiate(MovementBlocker, new Vector3(x, 0, y), Quaternion.identity, CollisionHolder);
				}
				if (walkable && floorReplacingEntity == null) {
					blocker = Instantiate(MovementBlocker, new Vector3(x, -1, y), Quaternion.identity, CollisionHolder);
				}
				if (blocker != null) blocker.name = $"Movement Blocker: ({x}, {y})";

				DrawWall(dg, tileSpec, 0, -1);
				DrawWall(dg, tileSpec, 0, 1);
				DrawWall(dg, tileSpec, -1, 0);
				DrawWall(dg, tileSpec, 1, 0);

				// Draw ceiling if it's a floorish tile or if the wall is shorter than the ceiling.
				if (tileSpec.DrawAsFloor || tileSpec.Terrain.WallHeight < tileSpec.CeilingOffset) {
					AddCeiling(tileSpec, ceilingReplacingEntity != null, dg, tileSpec.CeilingOffset);
				}

				if (tileSpec.Tile == Tile.Wall && tileSpec.Terrain.WallHeight < tileSpec.CeilingOffset && tileSpec.Terrain.WallCapDrawer != null) {
					GameObject go = tileSpec.Terrain.WallCapDrawer.DrawFlat(EnvironmentHolder, dg.ConsistentRNG, tileSpec);
					go.transform.localPosition = new Vector3(x, tileSpec.Terrain.WallHeight, y);
					go.name = $"Wall Cap: ({x}, {y})";
				}

				if (tileSpec.DrawAsFloor) {
					if (tileSpec.GroundOffset == 0 || walkable) {
						AddFloor(tileSpec, x, y, tileSpec.Tile, floorReplacingEntity != null, dg, 0);
					}

					if (tileSpec.GroundOffset != 0) {
						AddFloor(tileSpec, x, y, Tile.Wall, floorReplacingEntity != null, dg, -tileSpec.GroundOffset);
					}
				}

				float entityPosition = walkable ? 0 : (tileSpec.DrawAsFloor ? -tileSpec.GroundOffset: tileSpec.CeilingOffset);

				foreach (Entity entity in tileSpec.Entities) {
					AddEntity(entity, x, y, entity.Type.RaiseToCeiling ? tileSpec.CeilingOffset - 1 : entityPosition, dg.ConsistentRNG);
				}
			}

			if (MergeMeshes) {
				MeshMerge.MergeAll(EnvironmentHolder.gameObject);
				MeshMerge.MergeAll(StaticEntityHolder.gameObject);
			}
			OnCreate(dg);

			if (dg != null) {
				RenderSettings.fogDensity = dg.FogDensity;
				RenderSettings.fogColor = dg.FogColor;
				if (Camera.main != null) Camera.main.backgroundColor = dg.FogColor;
			} else {
				RenderSettings.fogDensity = 0.01F;
				Color fogColor = Color.white;
				RenderSettings.fogColor = fogColor;
			}
			SetFarPlaneFromFog();

			GeneratorRef.Value = dg;
		}

		public void ClearGeneratedTestDungeon() {
			_testHolder = this.transform.Find(TEST_HOLDER_NAME);
			if (_testHolder != null) {
				DestroyImmediate(_testHolder.gameObject);
			}
		}

		public void PrintForTest(TheDungeon dg) {
			ClearGeneratedTestDungeon();

			_testHolder = new GameObject(TEST_HOLDER_NAME).transform;
			_testHolder.SetParent(this.transform);
			MakeHolders(_testHolder);

			Print(dg);
		}

		/// <summary>
		/// Draws a wall facing away from the current block to the adjacent block.
		/// </summary>
		/// <param name="dg">Dungeon generator</param>
		/// <param name="tile">Tile representing the "wall" tile</param>
		/// <param name="xOffset">X offset to check against</param>
		/// <param name="yOffset">Y offset to check against</param>
		/// <param name="drawStandardWalls">Whether or not the tile being drawn, in and of itself, should have walls drawn around it.
		/// Cases where this is true: a wall tile. Cases where it's not: a floor tile or a "draw as floor" wall tile.
		/// This parameter being false does not affect the presence of lower walls (i.e. walls drawn from being next to a lower
		/// area) or upper walls (walls drawn from being next to an area with higher walls)</param>
		private void DrawWall(TheDungeon dg, TileSpec tile, int xOffset, int yOffset) {
			int adjX = tile.Coords.x + xOffset;
			int adjY = tile.Coords.y + yOffset;
			// Outside of map bounds
			if (!dg.Contains(adjX, adjY)) return;
			DrawWall(dg, dg.GetTileSpec(adjX, adjY), tile);
		}

		/// <summary>
		/// Draws a wall on tile in the direction of facingTile.
		/// </summary>
		private void DrawWall(TheDungeon dg, TileSpec tile, TileSpec facingTile) {
			bool drawStandardWalls = !facingTile.DrawAsFloor;

			int xOffset = tile.Coords.x - facingTile.Coords.x;
			int yOffset = tile.Coords.y - facingTile.Coords.y;
			int rot = xOffset == 0 ? (yOffset > 0 ? 0 : 180) : (xOffset > 0 ? 90 : 270);

			float ceilingOffset = facingTile.Terrain.CeilingOffset;
			float wallHeight = facingTile.Terrain.WallHeight;
			float adjWallHeight = tile.Terrain.WallHeight;
			float groundOffset = facingTile.Terrain.GroundOffset;
			float adjGroundOffset = tile.Terrain.GroundOffset;

			bool hasCeiling = dg.Trait != Trait.Ceilingless && dg.Trait != Trait.CeilinglessPit && facingTile.Terrain.HasCeiling;
			adjWallHeight = dg.Trait == Trait.CeilinglessPit ? -1 : adjWallHeight;

			TileSpec tileDrawStyle = dg.Trait == Trait.InvasiveWalls ? facingTile : tile;

			Vector3 loc = new Vector3(facingTile.Coords.x, 0, facingTile.Coords.y);

			if (tile.DrawAsFloor) {
				// Draw wall segments below the floor.
				DrawWallSingle(EnvironmentHolder, dg.ConsistentRNG, tile, loc, rot, -adjGroundOffset, -groundOffset);

				// Draw the wall segments up to the wall height.
				if (drawStandardWalls) {
					DrawWallSingle(EnvironmentHolder, dg.ConsistentRNG, tile, loc, rot, 0, wallHeight);
				}
			}

			// Draw any wall segments to reach the height of an adjacent wall (in the style of the adjacent wall).
			if (drawStandardWalls) {
				float start = tile.DrawAsFloor ? wallHeight : 0;
				DrawWallSingle(EnvironmentHolder, dg.ConsistentRNG, tileDrawStyle, loc, rot, Math.Max(start, adjWallHeight), Math.Min(ceilingOffset, wallHeight));
			}

			// Add higher walls if they have a tile from the bottom to come from (normal wall) or from the top (ceiling).
			if (tile.DrawAsFloor) {
				if (drawStandardWalls || hasCeiling) {
					DrawWallSingle(EnvironmentHolder, dg.ConsistentRNG, tileDrawStyle, loc, rot, ceilingOffset, adjWallHeight);
				}
			}
		}

		private void DrawWallSingle(Transform parent, IRandom rand, TileSpec style, Vector3 loc, float rot, float from, float to) {
			if (from >= to) return;
			style.Terrain.WallDrawer.DrawWall(parent, rand, style, loc, rot, from, to);
		}

		private void SetFarPlaneFromFog() {
			if (Camera.main == null) return;
			float dist = Camera.main.farClipPlane;
			if (!RenderSettings.fog) {
			} else if (RenderSettings.fogMode == FogMode.Linear) {
				dist = RenderSettings.fogEndDistance;
			} else if (RenderSettings.fogMode == FogMode.Exponential) {
				dist = Mathf.Log(1f / 0.0019f) / RenderSettings.fogDensity;
			} else if (RenderSettings.fogMode == FogMode.ExponentialSquared) {
				dist = Mathf.Sqrt(Mathf.Log(1f / 0.0019f)) / RenderSettings.fogDensity;
			}
			Camera.main.farClipPlane = dist;
		}

		private void MakeHolders(Transform parent) {
			GameObject envGO = new GameObject("Environment");
			EnvironmentHolder = envGO.transform;
			EnvironmentHolder.SetParent(parent);
			GameObject stentGO = new GameObject("Entities [Static]");
			StaticEntityHolder = stentGO.transform;
			StaticEntityHolder.SetParent(parent);
			GameObject entGO = new GameObject("Entities");
			EntityHolder = entGO.transform;
			EntityHolder.SetParent(parent);
			GameObject colGO = new GameObject("Collisions");
			CollisionHolder = colGO.transform;
			CollisionHolder.SetParent(parent);
		}

		private static string TEST_HOLDER_NAME = "TestHolder";


		protected virtual void OnCreate(TheDungeon generator) {

		}

		private void AddEntity(Entity entity, int x, int y, float z, IRandom rand) {
			Vector2Int coords = new Vector2Int(x, y);
			GameObject instantiatedObject = entity.Type.Initialize(coords, new Vector3(x, z, y));

			foreach (IEntityInit init in instantiatedObject.GetComponentsInChildren<IEntityInit>()) {
				init.DoInit(coords, rand);
			}

			instantiatedObject.transform.SetParent(entity.Type.CanBeMerged ? StaticEntityHolder : EntityHolder);
			instantiatedObject.name = $"Entity: {entity.Type.ID} @ ({x}, {y})";
			entity.Code?.Invoke(instantiatedObject, entity, rand);
		}

		private void AddCeiling(TileSpec tileSpec, bool entityReplacesCeiling, TheDungeon dg, float ceilZ = 1) {
			if (tileSpec.Terrain == null) return;
			bool ceilingless = dg.Trait == Trait.CeilinglessPit || dg.Trait == Trait.Ceilingless || !tileSpec.Terrain.HasCeiling;
			if (entityReplacesCeiling || ceilingless) return;

			GameObject go = tileSpec.Terrain.CeilingDrawer.DrawFlat(EnvironmentHolder, dg.ConsistentRNG, tileSpec);
			go.transform.localPosition = new Vector3(tileSpec.Coords.x, ceilZ - 1, tileSpec.Coords.y);
			go.name = $"Ceiling: ({tileSpec.Coords.x}, {tileSpec.Coords.y}) [{tileSpec.Terrain}]";
		}

		private void AddFloor(TileSpec tileSpec, int x, int y, Tile type, bool entityReplacesFloor, TheDungeon dg, float floorZ = 0) {
			if (entityReplacesFloor) return;

			if (tileSpec.Terrain == null) {
				return;
			}
			TerrainSO terrain = tileSpec.Terrain;

            GameObject go;
			if (type == Tile.Floor) {
				go = terrain.FloorDrawer.DrawFlat(EnvironmentHolder, dg.ConsistentRNG, tileSpec);
				go.name = $"Floor: ({x}, {y}) [{terrain}]";
			} else if (type == Tile.Wall) {
				go = terrain.WallFloorDrawer.DrawFlat(EnvironmentHolder, dg.ConsistentRNG, tileSpec);
				go.name = $"Floor [Lower]: ({x}, {y}) [{terrain}]";
			} else {
				// Unset tile.
				return;
			}

			go.transform.localPosition = new Vector3(x, floorZ, y);
			return;
		}

		public Vector3 PointForCoords(Vector2Int coords) {
			return transform.TransformPoint(new Vector3(coords.x, 0, coords.y));
		}

		public Vector2Int PointForLocalPoint(Vector3 localPos) {
			int x = (int)(localPos.x + 0.5F);
			int y = (int)(localPos.z + 0.5F);
			return new Vector2Int(x, y);
		}

		public Vector2Int PointForWorldPoint(Vector3 worldPoint) {
			return PointForLocalPoint(this.transform.InverseTransformPoint(worldPoint));
		}

		public TerrainSO TerrainForLocalPoint(Vector3 localPos) {
			Vector2Int pt = PointForLocalPoint(localPos);
			return _dungeon.GetTileSpec(pt).Terrain;
		}

		public AudioEvent FootstepForLocalPoint(Vector3 localPos) {
			TerrainSO terrain = TerrainForLocalPoint(localPos);
			return terrain.GetFootstepSound();
		}
	}
}