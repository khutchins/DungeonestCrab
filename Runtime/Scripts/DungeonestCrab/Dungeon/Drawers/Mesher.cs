using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.XR;

namespace DungeonestCrab.Dungeon.Printer {
    public class Mesher {
        private GameObject _host;
        private MeshRenderer _renderer;
        private MeshFilter _filter;
        private MeshCollider _collider;

        public class MeshInstance {
            public Material material;
            public List<Vector3> verts = new List<Vector3>();
            public List<Vector2> uvs = new List<Vector2>();
            public List<int> triangles = new List<int>();

            public MeshInstance(Material material) { this.material = material; }
            
            public Mesh MakeMesh() {
                Mesh mesh = new Mesh {
                    name = "Mesh",
                    vertices = verts.ToArray(),
                    triangles = triangles.ToArray(),
                    uv = uvs.ToArray()
                };
                mesh.RecalculateNormals();
                return mesh;
            }
        }

        private Dictionary<Material, MeshInstance> _instances = new Dictionary<Material, MeshInstance>();

        public Mesher(GameObject host, bool addCollider = false) {
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
        }

        private MeshInstance EnsureInstance(Material material) {
            if (_instances.TryGetValue(material, out MeshInstance instance)) {
                return instance;
            }
            MeshInstance newInstance = new MeshInstance(material);
            _instances[material] = newInstance;
            return newInstance;
        }

        /// <summary>
        /// Adds the vert with the given UV in (0, 1) space to texture view space.
        /// </summary>
        /// <param name="vert">Vertex</param>
        /// <param name="uv">UV in (0,1) space.</param>
        /// <param name="view">View to convert coordinates to.</param>
        /// <returns></returns>
        public int AddVert(ITextureView view, Vector3 vert, Vector2 uv) {
            return AddVert(view.Material, vert, view.ConvertToLocalUVSpace(uv));
        }

        public int AddVert(Material material, Vector3 vert, Vector2 uv) {
            var instance = EnsureInstance(material);
            int value = instance.verts.Count;
            instance.verts.Add(vert);
            instance.uvs.Add(uv);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddTriangle(ITextureView tv, int v1, int v2, int v3) {
            AddTriangle(tv.Material, v1, v2, v3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddTriangle(Material material, int v1, int v2, int v3) {
            var instance = EnsureInstance(material);
            instance.triangles.Add(v1);
            instance.triangles.Add(v2);
            instance.triangles.Add(v3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GenerateRect(ITextureView tv, int v1, int v2, int v3, int v4) {
            GenerateRect(tv.Material, v1, v2, v3, v4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GenerateRect(Material material, int v1, int v2, int v3, int v4) {
            var instance = EnsureInstance(material);
            instance.triangles.Add(v3);
            instance.triangles.Add(v2);
            instance.triangles.Add(v1);

            instance.triangles.Add(v3);
            instance.triangles.Add(v4);
            instance.triangles.Add(v2);
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
            if (_instances.Count == 1) {
                var entry = _instances.First().Value;
                Mesh mesh = entry.MakeMesh();

                _filter.mesh = mesh;
                _renderer.sharedMaterial = entry.material;
                if (_collider != null) {
                    _collider.sharedMesh = mesh;
                }
            } else {
                
                MeshInstance[] entries = _instances.Select(x => x.Value).ToArray();
                Material[] materials = new Material[entries.Length];
                var combine = new CombineInstance[entries.Length];
                for (int i = 0; i < entries.Length; i++) {
                    combine[i].mesh = entries[i].MakeMesh();
                    combine[i].subMeshIndex = 0;
                    materials[i] = entries[i].material;
                }

                _renderer.sharedMaterials = materials;

                Mesh mesh = new Mesh();
                mesh.name = "Combined Mesh";
                mesh.CombineMeshes(combine, false, false);
                mesh.RecalculateBounds();
                _filter.sharedMesh = mesh;
                if (_collider != null) {
                    _collider.sharedMesh = mesh;
                }
            }
            
        }
    }
}