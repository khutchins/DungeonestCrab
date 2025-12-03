using UnityEditor;
using UnityEngine;
using DungeonestCrab.Dungeon.Printer;
using DungeonestCrab.Dungeon;
using Pomerandomian;
using Bounds = UnityEngine.Bounds;
using System;

namespace DungeonestCrab.Editor {

    [CustomEditor(typeof(IFlatDrawer), true)]
    [CanEditMultipleObjects]
    public class FlatDrawerPreviewEditor : UnityEditor.Editor {

        private PreviewRenderUtility _previewUtility;
        private GameObject _previewInstance;

        private Vector2 _drag;
        private Vector2 _lightAngle = new Vector2(50, 50);
        private float _zoomFactor = 1.0f;

        private bool _isDirty = true;

        private bool _neighborN = false;
        private bool _neighborE = false;
        private bool _neighborS = false;
        private bool _neighborW = false;

        private void OnEnable() {
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnDisable() {
            Undo.undoRedoPerformed -= OnUndoRedo;

            if (_previewUtility != null) {
                _previewUtility.Cleanup();
                _previewUtility = null;
            }
            ClearPreviewInstance();
        }

        private void OnUndoRedo() {
            _isDirty = true;
            Repaint();
        }

        public override void OnInspectorGUI() {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck()) {
                _isDirty = true;
                Repaint();
            }
        }

        private void ClearPreviewInstance() {
            if (_previewInstance != null) {
                DestroyImmediate(_previewInstance);
                _previewInstance = null;
            }
        }

        public override bool HasPreviewGUI() => true;

        public override GUIContent GetPreviewTitle() => new GUIContent("Flat Drawer Preview");

        public override void OnPreviewSettings() {
            bool changed = false;

            GUILayout.Label("Neighbors:", EditorStyles.miniLabel);

            EditorGUI.BeginChangeCheck();

            _neighborN = GUILayout.Toggle(_neighborN, "N", EditorStyles.miniButtonMid, GUILayout.Width(25));
            _neighborS = GUILayout.Toggle(_neighborS, "S", EditorStyles.miniButtonMid, GUILayout.Width(25));
            _neighborE = GUILayout.Toggle(_neighborE, "E", EditorStyles.miniButtonMid, GUILayout.Width(25));
            _neighborW = GUILayout.Toggle(_neighborW, "W", EditorStyles.miniButtonMid, GUILayout.Width(25));

            GUILayout.Space(5);

            if (GUILayout.Button(new GUIContent("Reset", "Reset Camera"), EditorStyles.miniButton)) {
                _drag = Vector2.zero;
                _zoomFactor = 1.0f;
                _lightAngle = new Vector2(50, 50);
                changed = true;
            }

            if (EditorGUI.EndChangeCheck()) {
                changed = true;
            }

            if (changed) {
                _isDirty = true;
            }
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background) {
            if (_previewUtility == null) {
                _previewUtility = new PreviewRenderUtility();
                _previewUtility.camera.fieldOfView = 30f;
                _drag = new Vector2(0, -90);
            }

            HandleInput(r);

            if (_isDirty) {
                GeneratePreviewMesh();
                _isDirty = false;
            }

            if (_previewInstance == null) return;

            _previewUtility.BeginPreview(r, background);

            _previewUtility.lights[0].intensity = 1.0f;
            _previewUtility.lights[0].transform.rotation = Quaternion.Euler(_lightAngle.y, _lightAngle.x, 0);
            _previewUtility.ambientColor = new Color(0.5f, 0.5f, 0.5f, 1f);

            Bounds bounds = GetRenderBounds(_previewInstance);
            float objectSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z, 1f);
            float distance = objectSize * 4.5f * _zoomFactor;

            _previewUtility.camera.transform.position = new Vector3(0, 0, -distance);
            _previewUtility.camera.transform.rotation = Quaternion.identity;
            _previewUtility.camera.nearClipPlane = 0.1f;
            _previewUtility.camera.farClipPlane = distance * 20f;

            Quaternion rotation = Quaternion.Euler(_drag.y, -_drag.x, 0);
            Vector3 centerOffset = -bounds.center;

            Matrix4x4 pivotMatrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one) *
                                    Matrix4x4.TRS(centerOffset, Quaternion.identity, Vector3.one);

            _previewUtility.camera.Render();
            RenderGameObject(_previewInstance, pivotMatrix);

            _previewUtility.EndAndDrawPreview(r);
        }

        private void HandleInput(Rect r) {
            Event evt = Event.current;
            if (r.Contains(evt.mousePosition)) {
                if (evt.type == EventType.MouseDrag) {
                    if (evt.button == 0) {
                        _drag.x += evt.delta.x;
                        _drag.y -= evt.delta.y;
                    } else if (evt.button == 1) {
                        _lightAngle.x += evt.delta.x;
                        _lightAngle.y -= evt.delta.y;
                    }
                    evt.Use();
                    GUI.changed = true;
                } else if (evt.type == EventType.ScrollWheel) {
                    _zoomFactor += evt.delta.y * 0.05f;
                    _zoomFactor = Mathf.Clamp(_zoomFactor, 0.1f, 10f);
                    evt.Use();
                    GUI.changed = true;
                }
            }
        }

        private void GeneratePreviewMesh() {
            ClearPreviewInstance();

            IFlatDrawer drawer = target as IFlatDrawer;
            if (drawer == null) return;

            TileSpec centerTile = new TileSpec(Vector2Int.zero, Tile.Floor, ScriptableObject.CreateInstance<TerrainSO>(), false);
            MockAdjacency(centerTile, _neighborN, _neighborE, _neighborS, _neighborW);

            var info = new IFlatDrawer.FlatInfo {
                parent = null,
                random = new SystemRandom(12345),
                tileSpec = centerTile,
                tileSize = Vector3.one,
                hasCeiling = false
            };

            _previewInstance = drawer.DrawFlat(info);

            if (_previewInstance != null) {
                _previewInstance.hideFlags = HideFlags.HideAndDontSave;
                // Hide renderers so they don't show up in scene view.
                foreach (var renderer in _previewInstance.GetComponentsInChildren<Renderer>()) {
                    renderer.enabled = false;
                }
            }
        }

        private void MockAdjacency(TileSpec tile, bool n, bool e, bool s, bool w) {
            TileSpec.Adjacency adj = TileSpec.Adjacency.Here;

            if (n) adj |= TileSpec.Adjacency.N;
            if (e) adj |= TileSpec.Adjacency.E;
            if (s) adj |= TileSpec.Adjacency.S;
            if (w) adj |= TileSpec.Adjacency.W;

            tile.SetTileAdjacencies(adj);
            tile.SetTerrainAdjacencies(adj);
        }

        private Bounds GetRenderBounds(GameObject go) {
            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return new Bounds(Vector3.zero, Vector3.one);

            Bounds b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) {
                b.Encapsulate(renderers[i].bounds);
            }
            return b;
        }

        private void RenderGameObject(GameObject go, Matrix4x4 parentMatrix) {
            MeshFilter mf = go.GetComponent<MeshFilter>();
            MeshRenderer mr = go.GetComponent<MeshRenderer>();

            Matrix4x4 localMatrix = go.transform.localToWorldMatrix;
            Matrix4x4 finalMatrix = parentMatrix * localMatrix;

            if (mf != null && mr != null && mf.sharedMesh != null) {
                for (int i = 0; i < mf.sharedMesh.subMeshCount; i++) {
                    Material mat = (i < mr.sharedMaterials.Length) ? mr.sharedMaterials[i] : null;
                    if (mat != null) {
                        _previewUtility.DrawMesh(mf.sharedMesh, finalMatrix, mat, i);
                    }
                }
            }

            foreach (Transform child in go.transform) {
                RenderGameObject(child.gameObject, parentMatrix);
            }
        }
    }
}