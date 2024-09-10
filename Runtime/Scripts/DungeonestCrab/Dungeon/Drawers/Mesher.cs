using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    public class Mesher {
        private GameObject _host;
        private MeshRenderer _renderer;
        private MeshFilter _filter;
        private MeshCollider _collider;

        private List<Vector3> _verts = new List<Vector3>();
        private List<Vector2> _uvs = new List<Vector2>();
        private List<int> _triangles = new List<int>();

        public Mesher(GameObject host, Material material, bool addCollider = false) {
            _host = host;
            if (_host.TryGetComponent(out MeshFilter mf)) {
                _filter = mf;
            } else {
                _filter = _host.AddComponent<MeshFilter>();
            }
            if (_host.TryGetComponent(out MeshRenderer mr)) {
                _renderer = mr;
            } else {
                _renderer = _host.AddComponent<MeshRenderer>();
            }
            if (addCollider) {
                if (_host.TryGetComponent(out MeshCollider mc)) {
                    _collider = mc;
                } else {
                    _collider = _host.AddComponent<MeshCollider>();
                }
            }
            _renderer.sharedMaterial = material;
        }

        public int AddVert(Vector3 vert, Vector2 uv) {
            int value = _verts.Count;
            _verts.Add(vert);
            _uvs.Add(uv);
            return value;
        }

        public void GenerateRect(int v1, int v2, int v3, int v4) {
            _triangles.Add(v3);
            _triangles.Add(v2);
            _triangles.Add(v1);

            _triangles.Add(v3);
            _triangles.Add(v4);
            _triangles.Add(v2);
        }

        public static Vector3[] MakeRing(int sides, float offset) {
            sides = Mathf.Max(sides, 1);
            float anglePer = Mathf.PI * 2 / sides;
            Vector3[] ring = new Vector3[sides];

            for (int i = 0; i < sides; i++) {
                ring[i] = new Vector3(Mathf.Sin(-i * anglePer + offset), 0, Mathf.Cos(-i * anglePer + offset));
            }
            return ring;
        }

        public static Vector2[] DirectionalUV(int x, int y, int w, int h, int tx, int ty, int turns = 0) {
            float sx = x * 1f / tx;
            float ex = (x + w) * 1f / tx;
            float sy = 1 - (y * 1f / ty);
            float ey = 1 - ((y + h) * 1f / ty);

            Vector2[] unaltered = new Vector2[4] { new Vector2(sx, sy), new Vector2(ex, sy), new Vector2(ex, ey), new Vector2(sx, ey) };
            if (turns == 0) return unaltered;
            Vector2[] turned = new Vector2[4];
            for (int i = 0; i < 4; i++) {
                int mi = (i + turns) % 4;
                turned[i] = unaltered[mi];
            }
            return turned;
        }

        public void Finish() {
            Mesh mesh = new Mesh {
                name = "Mesh",
                vertices = _verts.ToArray(),
                triangles = _triangles.ToArray(),
                uv = _uvs.ToArray()
            };
            mesh.RecalculateNormals();

            _filter.mesh = mesh;
            if (_collider != null) {
                _collider.sharedMesh = mesh;
            }
        }
    }
}