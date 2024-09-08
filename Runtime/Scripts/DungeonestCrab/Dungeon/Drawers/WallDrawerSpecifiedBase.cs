using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Wall - Specified")]
    public abstract class WallDrawerSpecifiedBase : IWallDrawer {
        [SerializeField] Material Material;

        public override void DrawWall(WallInfo info) {
            // This is the base.
            GameObject wall = new GameObject($"Wall ({info.tileSpec.Coords.x}, {info.tileSpec.Coords.y})");
            wall.transform.SetParent(info.parent);

            Quaternion angle = Quaternion.AngleAxis(info.rotation, Vector3.up);
            Vector3 up = angle * new Vector3(0, info.tileSize.y, 0);
            Vector3 right = angle * new Vector3(info.tileSize.x, 0, 0);
            Vector3 back = angle * new Vector3(0, 0, info.tileSize.z);

            GameObject innerWall = new GameObject("DrawnWall");
            innerWall.transform.SetParent(wall.transform, false);
            DrawFromConfig(info, innerWall, up, right, back);
        }

        protected abstract WallSpec[] GetWallConfiguration();

        void DrawFromConfig(WallInfo info, GameObject wall, Vector3 up, Vector3 right, Vector3 back) {
            var renderer = wall.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = Material;
            var filter = wall.AddComponent<MeshFilter>();
            var mesh = new Mesh {
                name = "Wall"
            };
            Vector3 basePos = info.position + -right / 2f + back / 2f;
            int xVerts = 2;

            WallSpec[] wallSpecs = GetWallConfiguration();

            float leftMod = GetIsAdjacent(info.wallDraws, WallAdjacency.BottomLeft) 
                ? 1 
                : (GetIsAdjacent(info.wallDraws, WallAdjacency.Left) 
                    ? 0 
                    : GetIsAdjacent(info.wallDraws, WallAdjacency.TopLeft) ? -1 : 0);

            float rightMod = GetIsAdjacent(info.wallDraws, WallAdjacency.BottomRight)
                ? -1
                : (GetIsAdjacent(info.wallDraws, WallAdjacency.Right)
                    ? 0
                    : GetIsAdjacent(info.wallDraws, WallAdjacency.TopRight) ? 1 : 0);

            // Vertices & Mesh
            var vertices = new Vector3[xVerts * wallSpecs.Length];
            var uvs = new Vector2[xVerts * wallSpecs.Length];
            int idx = 0;
            for (float x = 0; x < xVerts; x++) {
                float xOffset = x == 0 ? leftMod : (x == xVerts - 1 ? rightMod : 0);
                for (int y = 0; y < wallSpecs.Length; y++) {
                    float compX = x + xOffset * wallSpecs[y].outset;
                    Vector3 xBase = basePos + compX * right;
                    Vector3 vPos = xBase + wallSpecs[y].percentY * up;
                    float outset = wallSpecs[y].outset;
                    vPos = vPos - back * outset;
                    vertices[idx] = vPos;
                    uvs[idx] = new Vector2(compX, wallSpecs[y].uvY);
                    ++idx;
                }
            }
            mesh.vertices = vertices;
            mesh.uv = uvs;

            // Triangles
            var triangles = new int[wallSpecs.Length * xVerts * 2 * 3];
            idx = -1;
            for (int x = 0; x < xVerts - 1; x++) {
                for (int y = 0; y < wallSpecs.Length - 1; y++) {
                    int baseNum = x * wallSpecs.Length + y;
                    triangles[++idx] = baseNum;
                    triangles[++idx] = baseNum + 1;
                    triangles[++idx] = baseNum + 1 + wallSpecs.Length;

                    triangles[++idx] = baseNum;
                    triangles[++idx] = baseNum + 1 + wallSpecs.Length;
                    triangles[++idx] = baseNum + wallSpecs.Length;
                }
            }
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            filter.mesh = mesh;
        }

        [System.Serializable]
        public class WallSpec {
            public float percentY = 0;
            public float outset = 0;
            public float uvY = 0;

            public WallSpec(float percentY, float outset, float uvY) {
                this.percentY = percentY;
                this.outset = outset;
                this.uvY = uvY;
            }
        }
    }
}