using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Wall - Specified Disjoint")]
    public class WallDrawerSpecifiedDisjoint : IWallDrawer {
        [SerializeField] WallSpec[] WallConfiguration;
        [SerializeField] Material Material;
        private WallSpec[] _cache = null;

        private void OnValidate() {
            _cache = null;
        }

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
            DrawFromConfig(innerWall, info.position, up, right, back, info.minY, info.maxY);
        }

        WallSpec[] EnsureCache() {
            if (_cache != null) return _cache;

            List<WallSpec> wallSpecs = WallConfiguration.ToList();

            List<WallSpec> generatedWallSpecs = new List<WallSpec>();

            for (int i = 0; i < wallSpecs.Count; i++) {
                WallSpec curr = wallSpecs[i];
                if (curr.endsUVY != curr.startsUVY) {
                    generatedWallSpecs.Add(new WallSpec(curr.percentY, curr.outset, curr.endsUVY, curr.endsUVY));
                }
                generatedWallSpecs.Add(curr);
            }
            _cache = generatedWallSpecs.ToArray();
            return _cache;
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

            WallSpec[] specs = EnsureCache();

            // Vertices & Mesh
            var vertices = new Vector3[xVerts * specs.Length];
            var uvs = new Vector2[xVerts * specs.Length];
            int idx = 0;
            for (float x = 0; x < xVerts; x++) {
                for (int y = 0; y < specs.Length; y++) {
                    Vector3 vPos = basePos + x * right + specs[y].percentY * up;
                    float perturb = specs[y].outset;
                    vPos = vPos - back * perturb;
                    vertices[idx] = vPos;
                    uvs[idx] = new Vector2(x, specs[y].startsUVY);
                    ++idx;
                }
            }
            mesh.vertices = vertices;
            mesh.uv = uvs;

            // Triangles
            var triangles = new int[specs.Length * xVerts * 2 * 3];
            idx = -1;
            for (int x = 0; x < xVerts - 1; x++) {
                for (int y = 0; y < specs.Length - 1; y++) {
                    int baseNum = x * specs.Length + y;
                    triangles[++idx] = baseNum;
                    triangles[++idx] = baseNum + 1;
                    triangles[++idx] = baseNum + 1 + specs.Length;

                    triangles[++idx] = baseNum;
                    triangles[++idx] = baseNum + 1 + specs.Length;
                    triangles[++idx] = baseNum + specs.Length;
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
            public float endsUVY = 0;
            public float startsUVY = 0;

            public WallSpec(float percentY, float outset, float endsUVY, float startsUVY) {
                this.percentY = percentY;
                this.outset = outset;
                this.endsUVY = endsUVY;
                this.startsUVY = startsUVY;
            }
        }
    }
}