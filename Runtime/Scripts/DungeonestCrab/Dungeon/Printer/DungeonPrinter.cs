﻿using System;
using System.Linq;
using UnityEngine;
using Pomerandomian;
using KH.Audio;
using KH.Tools;
using KH.Extensions;

namespace DungeonestCrab.Dungeon.Printer {
	[DefaultExecutionOrder(-1)]
	public class DungeonPrinter : MonoBehaviour {

		[SerializeField] DungeonReference GeneratorRef;
		[SerializeField] GameObject WalkableObject;
		[SerializeField] GameObject ImpassableObject;

		[Header("Config")]
		[Tooltip("Size of tile in (width, height, depth) order.")]
		[SerializeField] Vector3Int TileSize = Vector3Int.one;
		public bool MergeMeshes = true;

		public static DungeonPrinter Shared;

		private TheDungeon _dungeon;

		Transform EnvironmentHolder;
		Transform CollisionHolder;
		Transform StaticEntityHolder;
		Transform EntityHolder;
		private Transform _testHolder;
		private Vector2Int _tileFloorSize;
		private float _tileHeightMult;

		protected void Awake() {
			Shared = this;
			_tileFloorSize = TileSize.ToVectorXZ();
			_tileHeightMult = TileSize.y;
			MakeHolders(this.transform);
		}

		public void Print(TheDungeon dg) {
			_dungeon = dg;
			foreach (TileSpec tileSpec in dg.AllTiles()) {
				if (!IsDrawableTile(tileSpec)) {
					// Tile isn't configured. Means that it shouldn't be rendered.
					// The entities should still be drawn though.
					if (tileSpec.Entities.Count > 0) {
						Debug.LogWarning($"{tileSpec.Entities.Count} entities will not be drawn because they are on the invalid tile {tileSpec.Coords}");
					}
					continue;
				}
				int x = tileSpec.Coords.x, y = tileSpec.Coords.y;

				Entity floorReplacingEntity = tileSpec.Entities.FirstOrDefault(t => t.Type.ReplacesFloor);
				Entity ceilingReplacingEntity = tileSpec.Entities.FirstOrDefault(t => t.Type.ReplacesCeiling);

				bool walkable = tileSpec.Walkable;
				if (!walkable && ImpassableObject != null) {
					var blocker = Instantiate(ImpassableObject, OriginForCoords(tileSpec.Coords), Quaternion.identity, CollisionHolder);
					blocker.name = $"Wall Object: ({x}, {y})";
				}
				if (walkable && WalkableObject != null) {
					var blocker = Instantiate(WalkableObject, OriginForCoords(tileSpec.Coords), Quaternion.identity, CollisionHolder);
					blocker.name = $"Floor Object: ({x}, {y})";
				}

				DrawWall(dg, tileSpec, 0, -1);
				DrawWall(dg, tileSpec, 0, 1);
				DrawWall(dg, tileSpec, -1, 0);
				DrawWall(dg, tileSpec, 1, 0);

				// Draw ceiling if it's a floorish tile or if the wall is shorter than the ceiling.
				if (tileSpec.DrawAsFloor || tileSpec.Terrain.WallHeight < tileSpec.CeilingOffset) {
					AddCeiling(tileSpec, ceilingReplacingEntity != null, dg, tileSpec.CeilingOffset);
				}

				if (tileSpec.Tile == Tile.Wall && tileSpec.Terrain.WallHeight < tileSpec.CeilingOffset && tileSpec.Terrain.WallCapDrawer != null) {
					IFlatDrawer.FlatInfo info = new IFlatDrawer.FlatInfo {
						parent = EnvironmentHolder,
						random = dg.ConsistentRNG,
						tileSpec = tileSpec,
						tileSize = TileSize,
						hasCeiling = tileSpec.HasCeiling,
						ceilingHeight = tileSpec.CeilingOffset - tileSpec.Terrain.WallHeight,
					};
					GameObject go = tileSpec.Terrain.WallCapDrawer.DrawFlat(info);
					go.transform.SetParent(EnvironmentHolder, false);
					go.transform.localPosition = OriginForTile(tileSpec, tileSpec.Terrain.WallHeight);
					go.name = $"Wall Cap: ({x}, {y})";
				}

				if (tileSpec.DrawAsFloor) {
					if (tileSpec.GroundOffset == 0 || walkable) {
						AddFloor(tileSpec, tileSpec.Tile, floorReplacingEntity != null, dg, 0);
					}

					if (tileSpec.GroundOffset != 0) {
						AddFloor(tileSpec, Tile.Wall, floorReplacingEntity != null, dg, -tileSpec.GroundOffset);
					}
				}

				foreach (Entity entity in tileSpec.Entities) {
					float entityPosition = walkable ? 0 : -tileSpec.GroundOffset;
					AddEntity(dg, entity, tileSpec.Coords, entity.Type.RaiseToCeiling ? tileSpec.CeilingOffset - 1 : entityPosition, dg.ConsistentRNG);
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

			if (GeneratorRef != null) GeneratorRef.Value = dg;
		}

		public void ClearGeneratedTestDungeon() {
			_testHolder = this.transform.Find(TEST_HOLDER_NAME);
			if (_testHolder != null) {
				DestroyImmediate(_testHolder.gameObject);
			}
		}

		public void PrintForTest(TheDungeon dg) {
			ClearGeneratedTestDungeon();

			_tileFloorSize = TileSize.ToVectorXZ();
			_tileHeightMult = TileSize.y;

			_testHolder = new GameObject(TEST_HOLDER_NAME).transform;
			_testHolder.SetParent(this.transform);
			MakeHolders(_testHolder);

			Print(dg);
		}

		private bool IsDrawableTile(TileSpec tile) {
			return tile != null && tile.IsDrawable;
		}

		/// <summary>
		/// Draws a wall for the tile in the direction given by the offsets.
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
			TileSpec adjTile = dg.GetTileSpecSafe(tile.Coords + new Vector2Int(xOffset, yOffset));
			// Outside of map bounds or not properly configured.
			if (!IsDrawableTile(adjTile)) return;
			DrawWall(dg, tile, adjTile);
		}

		private bool DrawsStandardWalls(TileSpec tile, TileSpec adjTile) {
			return tile != null && adjTile != null && tile.DrawAdjacentWalls && adjTile.DrawWalls;
		}

		private Vector2Int LeftWallOffset(TileSpec tile, TileSpec adjTile) {
			Vector2 offset = adjTile.Coords - tile.Coords;
			var rotated = Quaternion.Euler(0, 0, -90) * offset;
			return Vector2Int.RoundToInt(rotated);
		}

		private Vector2Int RotatedV2I(Vector2Int vector, float rotation) {
            var rotated = Quaternion.Euler(0, 0, rotation) * new Vector2(vector.x, vector.y);
            return Vector2Int.RoundToInt(rotated);
        }

		/// <summary>
		/// Returns the adjacencies for the wall tile.
		/// It's packed as 
		/// XWX
		/// XFX
		/// Where F is the floor (always 0) and W is the wall (always 1).
		/// Packing order is:
		/// 012
		/// 345
		/// </summary>
		private int WallTileAdjacencies(TheDungeon dg, TileSpec tile, TileSpec adjTile) {
			Vector2Int offset = adjTile.Coords - tile.Coords;
			Vector2Int leftOffset = RotatedV2I(offset, 90);

			TileSpec left = dg.GetTileSpecSafe(tile.Coords + leftOffset);
			TileSpec leftForward = left != null ? dg.GetTileSpecSafe(left.Coords + offset) : null;
			TileSpec right = dg.GetTileSpecSafe(tile.Coords - leftOffset);
            TileSpec rightForward = right != null ? dg.GetTileSpecSafe(right.Coords + offset) : null;

            return IWallDrawer.PackWallAdjacencies(
                DrawsStandardWalls(leftForward, adjTile),
                DrawsStandardWalls(rightForward, adjTile),
                DrawsStandardWalls(left, leftForward),
                DrawsStandardWalls(right, rightForward),
				DrawsStandardWalls(tile, left),
				DrawsStandardWalls(tile, right)
            );
		}

		/// <summary>
		/// Draws a wall on tile in the direction of adjTile.
		/// </summary>
		private void DrawWall(TheDungeon dg, TileSpec tile, TileSpec adjTile) {
			bool drawStandardWalls = adjTile.DrawWalls;

			int xOffset = tile.Coords.x - adjTile.Coords.x;
			int yOffset = tile.Coords.y - adjTile.Coords.y;
			int rot = xOffset == 0 ? (yOffset > 0 ? 180 : 0) : (xOffset > 0 ? 270 : 90);

			float ceilingOffset = adjTile.Terrain.CeilingOffset * _tileHeightMult;
			float wallHeight = adjTile.Terrain.WallHeight * _tileHeightMult;
			float adjWallHeight = tile.Terrain.WallHeight * _tileHeightMult;
			float groundOffset = adjTile.Terrain.GroundOffset;
			float adjGroundOffset = tile.Terrain.GroundOffset;

			bool hasCeiling = dg.Trait != Trait.Ceilingless && dg.Trait != Trait.CeilinglessPit && adjTile.HasCeiling;
			adjWallHeight = dg.Trait == Trait.CeilinglessPit ? -1 : adjWallHeight;


			TileSpec tileDrawStyle = dg.Trait == Trait.InvasiveWalls ? tile : adjTile;

			Vector3 loc = OriginForTile(tile);

            IWallDrawer.WallInfo info = new IWallDrawer.WallInfo {
                parent = EnvironmentHolder,
                random = dg.ConsistentRNG,
                position = loc,
                rotation = rot,
                tileSize = TileSize,
            };

            if (tile.DrawAsFloor) {
                // Draw wall segments below the floor.
                info.tileSpec = tile;
                info.minY = -adjGroundOffset;
                info.maxY = -groundOffset;
                DrawWallSingle(info);
            }

            // The wall is only drawn if this tile is not itself a wall. 
            // Draw the wall segments up to the wall height.
            if (DrawsStandardWalls(tile, adjTile)) {
				info.tileSpec = tileDrawStyle;
				info.minY = 0;
				info.maxY = wallHeight;
				info.wallDraws = WallTileAdjacencies(dg, tile, adjTile);
                DrawWallSingle(info);
            }

			// Draw any wall segments to reach the height of an adjacent wall (in the style of the adjacent wall).
			if (drawStandardWalls) {
				float start = tile.DrawAsFloor ? wallHeight : 0;
                info.tileSpec = tileDrawStyle;
                info.minY = Math.Max(start, adjWallHeight);
				info.maxY = Math.Min(ceilingOffset, wallHeight);
                DrawWallSingle(info);
            }

			// Add higher walls if they have a tile from the bottom to come from (normal wall) or from the top (ceiling).
			if (tile.DrawAsFloor) {
				if (drawStandardWalls || hasCeiling) {
                    info.tileSpec = tileDrawStyle;
                    info.minY = ceilingOffset;
                    info.maxY = adjWallHeight;
                    DrawWallSingle(info);
				}
			}
		}

		private void DrawWallSingle(IWallDrawer.WallInfo info) {
			if (info.minY >= info.maxY) return;
			if (info.tileSpec.Terrain.WallDrawer != null) {
				info.tileSpec.Terrain.WallDrawer.DrawWall(info);
			}
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

		private void AddEntity(TheDungeon dungeon, Entity entity, Vector2Int coords, float z, IRandom rand) {
			GameObject instantiatedObject = entity.Type.Prefab != null ?
				Instantiate(entity.Type.Prefab)
				: new GameObject();
			instantiatedObject.transform.SetParent(entity.Type.CanBeMerged ? StaticEntityHolder : EntityHolder);
			instantiatedObject.transform.localPosition = OriginForCoords(coords) + new Vector3(0, z * _tileHeightMult, 0);
			instantiatedObject.transform.localEulerAngles = new Vector3(0, entity.YAngle, 0);

			foreach (IEntityInit init in instantiatedObject.GetComponentsInChildren<IEntityInit>()) {
				init.DoInit(instantiatedObject, entity, rand);
			}

			instantiatedObject.transform.SetParent(entity.Type.CanBeMerged ? StaticEntityHolder : EntityHolder);
			instantiatedObject.name = $"Entity: {entity.Type.ID} @ ({coords.x}, {coords.y})";
			entity.Code?.Invoke(dungeon, instantiatedObject, entity, rand);
		}

		private void AddCeiling(TileSpec tileSpec, bool entityReplacesCeiling, TheDungeon dg, float ceilZ = 1) {
			if (tileSpec.Terrain == null) return;
			bool ceilingless = dg.Trait == Trait.CeilinglessPit || dg.Trait == Trait.Ceilingless || !tileSpec.HasCeiling;
			if (entityReplacesCeiling || ceilingless) return;

			GameObject go = tileSpec.Terrain.CeilingDrawer.DrawFlat(new IFlatDrawer.FlatInfo { parent = EnvironmentHolder, random = dg.ConsistentRNG, tileSpec = tileSpec, tileSize = TileSize, hasCeiling = true, ceilingHeight = tileSpec.CeilingOffset + 1 });
            go.transform.SetParent(EnvironmentHolder, false);
            go.transform.localPosition = OriginForTile(tileSpec, ceilZ);
			go.transform.localScale = new Vector3(1, -1, 1);
			go.name = $"Ceiling: ({tileSpec.Coords.x}, {tileSpec.Coords.y}) [{tileSpec.Terrain}]";
		}

		private void AddFloor(TileSpec tileSpec, Tile type, bool entityReplacesFloor, TheDungeon dg, float floorZ = 0) {
			if (entityReplacesFloor) return;

			if (tileSpec.Terrain == null) {
				return;
			}
			TerrainSO terrain = tileSpec.Terrain;

            GameObject go;
			IFlatDrawer.FlatInfo flatInfo = new IFlatDrawer.FlatInfo {
				parent = EnvironmentHolder,
				random = dg.ConsistentRNG,
				tileSpec = tileSpec,
				tileSize = TileSize,
				hasCeiling = tileSpec.HasCeiling && dg.Trait != Trait.Ceilingless && dg.Trait != Trait.CeilinglessPit,
				ceilingHeight = tileSpec.CeilingOffset + 1,
			};
			if (tileSpec.DrawAsFloor) {
				IFlatDrawer drawer = type == Tile.Floor ? terrain.FloorDrawer : terrain.WallFloorDrawer;
				go = drawer.DrawFlat(flatInfo);
                go.transform.SetParent(EnvironmentHolder, false);
                go.name = $"Floor: ({tileSpec.Coords.x}, {tileSpec.Coords.y}) [{terrain}]";
			} else {
				// Unset tile.
				return;
			}

			go.transform.localPosition = OriginForTile(tileSpec, floorZ);
			return;
		}

		private Vector3 OriginForTile(TileSpec tile) {
			return OriginForCoords(tile.Coords);
		}

		private Vector3 OriginForTile(TileSpec tile, float zOffset) {
			return OriginForCoords(tile.Coords) + new Vector3(0, zOffset * _tileHeightMult, 0);
		}

		private Vector3 OriginForCoords(Vector2Int point) {
			return (point * _tileFloorSize).ToVectorX0Y();
		}

		public Vector3 PointForCoords(Vector2Int coords) {
			return transform.TransformPoint(new Vector3(coords.x, 0, coords.y));
		}

		public Vector2Int PointForLocalPoint(Vector3 localPos) {
			return new Vector2Int(
				(int)((localPos.x + 0.5f) * _tileFloorSize.x),
				(int)((localPos.z + 0.5f) * _tileFloorSize.y));
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