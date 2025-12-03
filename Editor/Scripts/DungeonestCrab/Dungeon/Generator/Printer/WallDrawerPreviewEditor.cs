using UnityEditor;
using UnityEngine;
using DungeonestCrab.Dungeon.Printer;
using DungeonestCrab.Dungeon;
using Pomerandomian;
using System;
using Bounds = UnityEngine.Bounds;

namespace DungeonestCrab.Editor {

    [CustomEditor(typeof(IWallDrawer), true)]
    [CanEditMultipleObjects]
    public class WallDrawerPreviewEditor : UnityEditor.Editor {

        private PreviewRenderUtility _previewUtility;
        private GameObject _previewInstance;

        private Vector2 _drag;
        private Vector2 _lightAngle = new Vector2(50, 50);
        private float _zoomFactor = 1.0f;

        private bool _isDirty = true;

        private bool _adjTL = false;
        private bool _adjL = true;
        private bool _adjBL = false;

        private bool _adjTR = false;
        private bool _adjR = true;
        private bool _adjBR = false;

        private float _wallHeight = 1.0f;

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

        public override void OnPreviewSettings() {
            bool changed = false;

            if (GUILayout.Button(new GUIContent("R", "Reset Camera"), EditorStyles.miniButton, GUILayout.Width(20))) {
                _drag = new Vector2(0, -20);
                _zoomFactor = 1.0f;
                _lightAngle = new Vector2(50, 50);
                changed = true;
            }

            GUILayout.Space(5);

            EditorGUI.BeginChangeCheck();

            GUILayout.Label("T", EditorStyles.miniLabel);
            _adjTL = GUILayout.Toggle(_adjTL, new GUIContent("", "Top Left Neighbor"), EditorStyles.miniButton, GUILayout.Width(15), GUILayout.Height(15));
            _adjTR = GUILayout.Toggle(_adjTR, new GUIContent("", "Top Right Neighbor"), EditorStyles.miniButton, GUILayout.Width(15), GUILayout.Height(15));

            GUILayout.Space(2);

            GUILayout.Label("M", EditorStyles.miniLabel);
            _adjL = GUILayout.Toggle(_adjL, new GUIContent("", "Left Neighbor"), EditorStyles.miniButton, GUILayout.Width(15), GUILayout.Height(15));
            _adjR = GUILayout.Toggle(_adjR, new GUIContent("", "Right Neighbor"), EditorStyles.miniButton, GUILayout.Width(15), GUILayout.Height(15));

            GUILayout.Space(2);

            GUILayout.Label("B", EditorStyles.miniLabel);
            _adjBL = GUILayout.Toggle(_adjBL, new GUIContent("", "Bottom Left Neighbor"), EditorStyles.miniButton, GUILayout.Width(15), GUILayout.Height(15));
            _adjBR = GUILayout.Toggle(_adjBR, new GUIContent("", "Bottom Right Neighbor"), EditorStyles.miniButton, GUILayout.Width(15), GUILayout.Height(15));

            GUILayout.Space(10);

            GUILayout.Label($"Height", EditorStyles.miniLabel);
            _wallHeight = EditorGUILayout.FloatField(_wallHeight, GUILayout.Width(30));

            if (EditorGUI.EndChangeCheck()) changed = true;

            if (changed) _isDirty = true;
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background) {
            if (_previewUtility == null) {
                _previewUtility = new PreviewRenderUtility();
                _previewUtility.camera.fieldOfView = 30f;
                _drag = new Vector2(0, -20);
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
            float distance = objectSize * 3.5f * _zoomFactor;

            _previewUtility.camera.transform.position = new Vector3(0, 0, -distance);
            _previewUtility.camera.transform.rotation = Quaternion.identity;
            _previewUtility.camera.nearClipPlane = 0.1f;
            _previewUtility.camera.farClipPlane = distance * 20f;

            Quaternion rotation = Quaternion.Euler(_drag.y, -_drag.x, 0);
            Vector3 centerOffset = -bounds.center;

            Matrix4x4 pivotMatrix =
                Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one) *
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

            IWallDrawer drawer = target as IWallDrawer;
            if (drawer == null) return;

            TileSpec mockTile = new TileSpec(Vector2Int.zero, Tile.Wall, ScriptableObject.CreateInstance<TerrainSO>(), false);

            int packedAdjacencies = IWallDrawer.PackWallAdjacencies(
                _adjTL, _adjTR,
                _adjL, _adjR,
                _adjBL, _adjBR
            );

            IWallDrawer.WallInfo info = new IWallDrawer.WallInfo {
                parent = null,
                random = new SystemRandom(12345),
                tileSpec = mockTile,
                position = Vector3.zero,
                rotation = 0f,
                tileSize = new Vector3Int(1, 1, 1),
                wallDraws = packedAdjacencies,
                wallSameTerrainDraws = packedAdjacencies,
                minY = 0,
                maxY = _wallHeight
            };

            // We need a root object to hold the result because DrawWall takes a parent
            _previewInstance = new GameObject("PreviewRoot");
            _previewInstance.hideFlags = HideFlags.HideAndDontSave;
            info.parent = _previewInstance.transform;

            // Draw
            drawer.DrawWall(info);

            if (_previewInstance != null) {
                foreach (var renderer in _previewInstance.GetComponentsInChildren<Renderer>()) {
                    renderer.enabled = false;
                }
            }
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