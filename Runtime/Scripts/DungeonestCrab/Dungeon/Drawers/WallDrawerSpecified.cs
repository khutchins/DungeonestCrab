using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Wall - Specified")]
    public class WallDrawerSpecified : IWallDrawer {
        [SerializeField] WallSpec[] WallConfiguration;
        [SerializeField] Material Material;

        public override void DrawWall(Transform parent, IRandom random, TileSpec tile, Vector3 position, Vector3Int tileSize, float rot, float minY, float maxY) {
            // This is the base.
            GameObject wall = new GameObject($"Wall ({tile.Coords.x}, {tile.Coords.y})");
            wall.transform.SetParent(parent);

            Quaternion angle = Quaternion.AngleAxis(rot, Vector3.up);
            Vector3 up = angle * new Vector3(0, tileSize.y, 0);
            Vector3 right = angle * new Vector3(tileSize.x, 0, 0);
            Vector3 back = angle * new Vector3(0, 0, tileSize.z);

            GameObject innerWall = new GameObject("DrawnWall");
            innerWall.transform.SetParent(wall.transform, false);
            DrawFromConfig(innerWall, position, up, right, back, minY, maxY);
        }

        protected virtual WallSpec[] GetWallConfiguration() {
            return WallConfiguration;
        }

        void DrawFromConfig(GameObject wall, Vector3 position, Vector3 up, Vector3 right, Vector3 back, float minY, float maxY) {
            var renderer = wall.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = Material;
            var filter = wall.AddComponent<MeshFilter>();
            var mesh = new Mesh {
                name = "Wall"
            };
            Vector3 basePos = position + -right / 2f + back / 2f;
            int xVerts = 2;

            WallSpec[] wallSpecs = GetWallConfiguration();

            // Vertices & Mesh
            var vertices = new Vector3[xVerts * wallSpecs.Length];
            var uvs = new Vector2[xVerts * wallSpecs.Length];
            int idx = 0;
            for (float x = 0; x < xVerts; x++) {
                for (int y = 0; y < wallSpecs.Length; y++) {
                    Vector3 vPos = basePos + x * right + wallSpecs[y].percentY * up;
                    float perturb = wallSpecs[y].outset;
                    vPos = vPos - back * perturb;
                    vertices[idx] = vPos;
                    uvs[idx] = new Vector2(x, wallSpecs[y].uvY);
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
        }
    }
}