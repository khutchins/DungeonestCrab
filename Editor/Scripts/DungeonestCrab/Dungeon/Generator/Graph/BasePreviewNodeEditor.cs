using DungeonestCrab.Dungeon.Generator.Graph;
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using static DungeonestCrab.Dungeon.Generator.Graph.BasePreviewNode;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CustomNodeEditor(typeof(BasePreviewNode))]
    public class BasePreviewNodeEditor : NodeEditor {

        public override void OnBodyGUI() {
            base.OnBodyGUI();

            BasePreviewNode node = target as BasePreviewNode;

            GUILayout.Space(5);

            // Foldout
            EditorGUI.BeginChangeCheck();
            bool expanded = EditorGUILayout.Foldout(node.ShowPreview, "Preview", true, EditorStyles.foldoutHeader);
            if (EditorGUI.EndChangeCheck()) {
                node.ShowPreview = expanded;
                serializedObject.ApplyModifiedProperties();

                // If they unfolded it, they probably want an up-to-date preview.
                if (expanded) {
                    node.UpdatePreview();
                }
            }

            if (expanded) {
                EditorGUI.BeginChangeCheck();
                PreviewMode newMode = (PreviewMode)EditorGUILayout.EnumPopup(node.ViewMode);
                if (EditorGUI.EndChangeCheck()) {
                    node.ViewMode = newMode;
                    node.UpdatePreview();
                }

                if (node.PreviewTexture == null && Event.current.type == EventType.Layout) {
                    node.UpdatePreview();
                }
                Texture2D tex = node.PreviewTexture;

                if (tex != null) {
                    float aspect = (float)tex.width / tex.height;
                    float targetWidth = 200;
                    float targetHeight = targetWidth / aspect;

                    Rect rect = GUILayoutUtility.GetRect(targetWidth, targetHeight);

                    if (Event.current.type == EventType.Repaint) {
                        GUI.DrawTexture(rect, tex, ScaleMode.ScaleToFit, true);
                    }

                    var style = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                    style.alignment = TextAnchor.MiddleRight;
                    GUILayout.Label($"{tex.width}x{tex.height}", style);
                } else {
                    if (GUILayout.Button("Generate Preview")) {
                        node.UpdatePreview();
                    }
                }

                if (GUILayout.Button("Refresh")) {
                    node.UpdatePreview();
                }
            }
        }
    }
}