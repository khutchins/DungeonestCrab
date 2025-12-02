using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;
using KH.Audio;
using KH.Tools;
using KH.Extensions;
using DungeonestCrab.Dungeon.Audio;

namespace DungeonestCrab.Dungeon.Printer {

    public class DungeonInfoMixin : TheDungeonMixin {
        private readonly DungeonPrinter _printer;

        public DungeonInfoMixin(DungeonPrinter printer) {
            _printer = printer;
        }

        public Vector2Int PointForWorldPoint(Vector3 worldPoint) {
            if (_printer == null) {
                // Probably accessing after scene change.
                return Vector2Int.zero;
            }
            return _printer.PointForWorldPoint(worldPoint);
        }

        public Vector3 WorldPointForPoint(Vector2Int coords) {
            if (_printer == null) {
                // Probably accessing after scene change.
                return Vector3.zero;
            }
            return _printer.PointForCoords(coords);
        }
    }

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
        public TheDungeon DungeonInstance => _dungeon;

        // Transform Holders
        public Transform EnvironmentHolder { get; private set; }
        public Transform CollisionHolder { get; private set; }
        public Transform StaticEntityHolder { get; private set; }
        public Transform EntityHolder { get; private set; }

        private Transform _testHolder;
        private Vector2Int _tileFloorSize;
        private float _tileHeightMult;

        protected void Awake() {
            // Clear any existing generator value.
            if (GeneratorRef != null) GeneratorRef.Value = null;
            Shared = this;
            _tileFloorSize = TileSize.ToVectorXZ();
            _tileHeightMult = TileSize.y;
            MakeHolders(this.transform);
        }

        public void Print(TheDungeon dg) {
            _dungeon = dg;

            dg.Mixins.Add(new DungeonInfoMixin(this));

            // Decorator pre-pass.
            if (dg.Traits != null) {
                foreach (var trait in dg.Traits) {
                    trait.DecorateGlobal(dg, this, EnvironmentHolder);
                }
            }

            foreach (TileSpec tileSpec in dg.AllTiles()) {
                if (!IsDrawableTile(tileSpec)) {
                    // Tile isn't configured. Means that it shouldn't be rendered.
                    // The entities should still be drawn though.
                    if (tileSpec.Entities.Count > 0) {
                        Debug.LogWarning($"{tileSpec.Entities.Count} entities will not be drawn because they are on the invalid tile {tileSpec.Coords}");
                    }
                    continue;
                }

                // Establish defaults from Terrain
                var ruleConfig = new TileRuleConfig(tileSpec);

                // Apply Trait Overrides
                if (dg.Traits != null) {
                    foreach (var trait in dg.Traits) {
                        trait.ModifyTileRules(tileSpec, ref ruleConfig);
                    }
                }

                // This notably does not check entities when placing the collider.
                // The expectation (as of now, when I'm rewriting Deeper and Deeper)
                // is that entities that block a tile will have their own collider,
                // because otherwise this collider would eat the interactions.
                bool basicWalkable = tileSpec.Tile == Tile.Floor;
                Vector3 origin = OriginForCoords(tileSpec.Coords);
                if (!basicWalkable && ImpassableObject != null) {
                    var blocker = Instantiate(ImpassableObject, origin, Quaternion.identity, CollisionHolder);
                    blocker.name = $"Wall Collider: {tileSpec.Coords}";
                }
                if (basicWalkable && !tileSpec.Entities.Any(x => x.Type.ReplacesFloor) && WalkableObject != null) {
                    var blocker = Instantiate(WalkableObject, origin, Quaternion.identity, CollisionHolder);
                    blocker.name = $"Floor Collider: {tileSpec.Coords}";
                }

                DrawWall(dg, tileSpec, 0, -1, ruleConfig);
                DrawWall(dg, tileSpec, 0, 1, ruleConfig);
                DrawWall(dg, tileSpec, -1, 0, ruleConfig);
                DrawWall(dg, tileSpec, 1, 0, ruleConfig);

                // Draw ceiling if it's a floorish tile or if the wall is shorter than the ceiling.
                if (tileSpec.DrawAsFloor || ruleConfig.WallHeight < ruleConfig.CeilingHeight) {
                    if (ruleConfig.DrawCeiling && !EntityReplaces(tileSpec, t => t.ReplacesCeiling)) {
                        AddCeiling(tileSpec, ruleConfig.CeilingHeight);
                    }
                }

                bool walkable = tileSpec.Walkable;

                // Draw Wall Cap (Top of a short wall)
                if (tileSpec.Tile == Tile.Wall && ruleConfig.WallHeight < ruleConfig.CeilingHeight && tileSpec.Terrain.WallCapDrawer != null) {
                    IFlatDrawer.FlatInfo info = new IFlatDrawer.FlatInfo {
                        parent = EnvironmentHolder,
                        random = dg.ConsistentRNG,
                        tileSpec = tileSpec,
                        tileSize = TileSize,
                        hasCeiling = ruleConfig.DrawCeiling,
                        ceilingHeight = ruleConfig.CeilingHeight - ruleConfig.WallHeight,
                    };
                    GameObject go = tileSpec.Terrain.WallCapDrawer.DrawFlat(info);
                    go.transform.SetParent(EnvironmentHolder, false);
                    go.transform.localPosition = OriginForTile(tileSpec, ruleConfig.WallHeight);
                    go.name = $"Wall Cap: {tileSpec.Coords}";
                }

                if (tileSpec.DrawAsFloor) {
                    bool replacesFloor = EntityReplaces(tileSpec, t => t.ReplacesFloor);

                    // Draw floor at Zero (if walkable or default)
                    if (ruleConfig.GroundOffset == 0 || tileSpec.Walkable) {
                        AddFloor(tileSpec, tileSpec.Tile, replacesFloor, dg, 0);
                    }
                    // Draw sunken floor (if offset)
                    if (ruleConfig.GroundOffset != 0) {
                        AddFloor(tileSpec, Tile.Wall, replacesFloor, dg, -ruleConfig.GroundOffset);
                    }
                }

                foreach (Entity entity in tileSpec.Entities) {
                    float entityZ = walkable ? 0 : -ruleConfig.GroundOffset;
                    if (entity.Type.RaiseToCeiling) entityZ = ruleConfig.CeilingHeight - 1;

                    AddEntity(dg, entity, tileSpec.Coords, entityZ, dg.ConsistentRNG);
                }

                if (dg.Traits != null) {
                    foreach (var trait in dg.Traits) {
                        trait.DecorateTile(this, tileSpec, origin, EnvironmentHolder);
                    }
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
        private void DrawWall(TheDungeon dg, TileSpec tile, int xOffset, int yOffset, TileRuleConfig myRules) {
            TileSpec adjTile = dg.GetTileSpecSafe(tile.Coords + new Vector2Int(xOffset, yOffset));
            // Outside of map bounds or not properly configured.
            if (!IsDrawableTile(adjTile)) return;

            // We must apply traits to the neighbor to see if *their* geometry changed.
            var adjRules = new TileRuleConfig(adjTile);
            if (dg.Traits != null) {
                foreach (var t in dg.Traits) t.ModifyTileRules(adjTile, ref adjRules);
            }

            var wallConfig = new WallStyleConfig(adjTile);

            if (dg.Traits != null) {
                foreach (var t in dg.Traits) {
                    t.ResolveWallStyle(tile, adjTile, ref wallConfig);
                }
            }

            DrawWallCore(dg, tile, adjTile, xOffset, yOffset, myRules, adjRules, wallConfig);
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

        private void DrawWallCore(
            TheDungeon dg, TileSpec tile, TileSpec adjTile, int xOffset, int yOffset, 
                TileRuleConfig myRules, TileRuleConfig adjRules, WallStyleConfig wallStyle) {
            // Determine Rotation based on direction
            int rot = xOffset == 0 ? (yOffset > 0 ? 0 : 180) : (xOffset > 0 ? 90 : 270);
            Vector3 loc = OriginForTile(tile);

            IWallDrawer.WallInfo info = new IWallDrawer.WallInfo {
                parent = EnvironmentHolder,
                random = dg.ConsistentRNG,
                position = loc,
                rotation = rot,
                tileSize = TileSize,
            };

            // If I am a floor, and my neighbor is "deeper" than me (or I am floating), 
            // I need to draw a wall downwards.
            if (tile.DrawAsFloor) {
                if (adjRules.GroundOffset > myRules.GroundOffset) {
                    info.tileSpec = tile;
                    info.minY = -adjRules.GroundOffset;
                    info.maxY = -myRules.GroundOffset;
                    DrawWallSingle(info);
                }
            }

            // The wall is only drawn if this tile is not itself a wall. 
            // Draw the wall segments up to the wall height.
            if (DrawsStandardWalls(tile, adjTile)) {
                info.tileSpec = wallStyle.StyleSource; // Use Resolved Style
                info.minY = 0;
                info.maxY = adjRules.WallHeight * _tileHeightMult;
                info.wallDraws = WallTileAdjacencies(dg, tile, adjTile);
                DrawWallSingle(info);
            }

            // Add higher walls if they have a tile from the bottom to come from (normal wall) or from the top (ceiling).
            if (tile.DrawAsFloor) {
                if (adjTile.DrawWalls) {
                    float startHeight = adjRules.WallHeight * _tileHeightMult;
                    float endHeight = adjRules.CeilingHeight * _tileHeightMult;

                    if (endHeight > startHeight) {
                        info.tileSpec = wallStyle.StyleSource;
                        info.minY = startHeight;
                        info.maxY = endHeight;
                        DrawWallSingle(info);
                    }
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
            EnvironmentHolder = CreateChild(parent, "Environment");
            StaticEntityHolder = CreateChild(parent, "Entities [Static]");
            EntityHolder = CreateChild(parent, "Entities");
            CollisionHolder = CreateChild(parent, "Collisions");
        }

        private static readonly string TEST_HOLDER_NAME = "TestHolder";

        protected virtual void OnCreate(TheDungeon generator) { }

        private Transform CreateChild(Transform parent, string name) {
            Transform t = new GameObject(name).transform;
            t.SetParent(parent);
            t.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            return t;
        }

        private void AddEntity(TheDungeon dungeon, Entity entity, Vector2Int coords, float z, IRandom rand) {
            Transform parent = entity.Type.CanBeMerged ? StaticEntityHolder : EntityHolder;
            Vector3 pos = OriginForCoords(coords) + new Vector3(0, z * _tileHeightMult, 0);
            Quaternion rot = Quaternion.Euler(0, entity.YAngle, 0);

            GameObject instantiatedObject;

            if (entity.Type.Prefab != null) {
                instantiatedObject = Instantiate(entity.Type.Prefab, pos, rot, parent);
            } else {
                // I don't remember why I have this. Maybe so it doesn't break later steps?
                instantiatedObject = new GameObject("EntityRoot");
                instantiatedObject.transform.SetParent(parent, false);
                instantiatedObject.transform.SetPositionAndRotation(pos, rot);
            }

            foreach (IEntityInit init in instantiatedObject.GetComponentsInChildren<IEntityInit>()) {
                init.DoInit(instantiatedObject, entity, rand);
            }

            instantiatedObject.name = $"Entity: {entity.Type.ID} @ {coords}";
            entity.Code?.Invoke(dungeon, instantiatedObject, entity, rand);
        }

        private void AddCeiling(TileSpec tileSpec, float heightUnits) {
            if (tileSpec.Terrain == null) return;

            GameObject go = tileSpec.Terrain.CeilingDrawer.DrawFlat(new IFlatDrawer.FlatInfo {
                parent = EnvironmentHolder,
                random = _dungeon.ConsistentRNG,
                tileSpec = tileSpec,
                tileSize = TileSize,
                hasCeiling = true,
                ceilingHeight = heightUnits
            });

            go.transform.SetParent(EnvironmentHolder, false);
            go.transform.localPosition = OriginForTile(tileSpec, heightUnits);
            go.transform.localScale = new Vector3(1, -1, 1);
            go.name = $"Ceiling: {tileSpec.Coords}";
        }

        private void AddFloor(TileSpec tileSpec, Tile type, bool entityReplacesFloor, TheDungeon dg, float floorZ) {
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
                hasCeiling = tileSpec.HasCeiling, // Use cached?
                ceilingHeight = tileSpec.CeilingOffset + 1,
            };

            IFlatDrawer drawer = type == Tile.Floor ? terrain.FloorDrawer : terrain.WallFloorDrawer;
            if (drawer == null) return;

            go = drawer.DrawFlat(flatInfo);
            go.transform.SetParent(EnvironmentHolder, false);
            go.name = $"Floor: {tileSpec.Coords} [{terrain}]";
            go.transform.localPosition = OriginForTile(tileSpec, floorZ);
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
            return transform.TransformPoint(OriginForCoords(coords));
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
            if (terrain != null) {
                var mixin = terrain.GetMixin<TerrainAudioMixin>();
                if (mixin != null) {
                    return mixin.GetFootstepSound();
                }
            }
            return null;
        }

        private bool DrawsStandardWalls(TileSpec tile, TileSpec adjTile) {
            return tile != null && adjTile != null && tile.DrawAdjacentWalls && adjTile.DrawWalls;
        }

        private bool EntityReplaces(TileSpec tile, System.Func<EntitySpec, bool> pred) {
            return tile.Entities.Any(e => pred(e.Type));
        }
    }
}