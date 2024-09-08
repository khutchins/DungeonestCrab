using DungeonestCrab.Dungeon.Printer;
using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Wall - Perturbed")]
    public class WallDrawerCave : IWallDrawer {
        [SerializeField] int VerticesPerSide = 5;
        [SerializeField] bool ConvergeAtEdges = false;
        [Range(0, 0.5f)][SerializeField] float MaxInset = 0.1f;
        [SerializeField] Material Material;

        public override void DrawWall(WallInfo info) {
            // This is the base.
            GameObject wall = new GameObject($"Wall ({info.tileSpec.Coords.x}, {info.tileSpec.Coords.y})");
            wall.transform.SetParent(info.parent);
            //wall.transform.localPosition = new Vector3(position.x, 0, position.z);
            //wall.transform.localEulerAngles = new Vector3(0, rot, 0);

            Quaternion angle = Quaternion.AngleAxis(info.rotation, Vector3.up);
            Vector3 up = angle * new Vector3(0, info.tileSize.y, 0);
            Vector3 right = angle * new Vector3(info.tileSize.x, 0, 0);
            Vector3 back = angle * new Vector3(0, 0, info.tileSize.z);

            // Convert this back to being on the outside, as having the actual hard position will allow me to do some sort of hash to determine a position.
            GameObject innerWall = new GameObject("PerturbedWall");
            innerWall.transform.SetParent(wall.transform, false);
            VerticesPerSide = Mathf.Max(VerticesPerSide, 2);
            DrawPerturbedWall(info, innerWall, up, right, back);
        }

        void DrawPerturbedWall(WallInfo info, GameObject wall, Vector3 up, Vector3 right, Vector3 back) {
            var renderer = wall.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = Material;
            var filter = wall.AddComponent<MeshFilter>();
            var mesh = new Mesh {
                name = "Wall"
            };
            Vector3 basePos = info.position + -right / 2f + back / 2f;
            Vector3 xDelta = right / (VerticesPerSide - 1);
            Vector3 yDelta = up / (VerticesPerSide - 1);

            float leftMod = GetLeftMod(info.wallDraws);
            float rightMod = GetRightMod(info.wallDraws);

            // Vertices & Mesh
            var vertices = new Vector3[VerticesPerSide * VerticesPerSide];
            var uvs = new Vector2[VerticesPerSide * VerticesPerSide];
            int idx = 0;
            for (float x = 0; x < VerticesPerSide; x++) {
                float xOffset = x == 0 ? leftMod : (x == VerticesPerSide - 1 ? rightMod : 0);
                for (float y = 0; y < VerticesPerSide; y++) {
                    bool perturb = !(ConvergeAtEdges && (x == 0 || y == 0 || x == VerticesPerSide - 1 || y == VerticesPerSide - 1));
                    Vector3 vPos = basePos + x * xDelta + y * yDelta;
                    float inset = perturb ? InsetForPoint(vPos) : 0;
                    vPos = !perturb ? vPos : vPos - back * inset + inset * xOffset * right;
                    vertices[idx] = vPos;
                    uvs[idx] = new Vector2(x / (VerticesPerSide - 1), y / (VerticesPerSide - 1));
                    ++idx;
                }
            }
            mesh.vertices = vertices;
            mesh.uv = uvs;

            // Triangles
            var triangles = new int[(VerticesPerSide - 1) * (VerticesPerSide - 1) * 2 * 3];
            idx = -1;
            for (int x = 0; x < VerticesPerSide - 1; x++) {
                for (int y = 0; y < VerticesPerSide - 1; y++) {
                    int baseNum = y * VerticesPerSide + x;
                    triangles[++idx] = baseNum;
                    triangles[++idx] = baseNum + 1;
                    triangles[++idx] = baseNum + 1 + VerticesPerSide;

                    triangles[++idx] = baseNum;
                    triangles[++idx] = baseNum + 1 + VerticesPerSide;
                    triangles[++idx] = baseNum + VerticesPerSide;
                }
            }
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            filter.mesh = mesh;
        }

        float InsetForPoint(Vector3 position) {
            int modVert = VerticesPerSide - 1;
            float modifiedPos = (position.x * modVert * 13 + position.y * modVert * 29 + position.z * modVert * 113);
            return modifiedPos / 811f % 1 * MaxInset;
        }
    }
}