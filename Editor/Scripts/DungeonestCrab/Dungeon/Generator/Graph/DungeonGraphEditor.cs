using DungeonestCrab.Dungeon.Generator.Graph;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CustomNodeGraphEditor(typeof(DungeonGraph))]
    public class DungeonGraphEditor : NodeGraphEditor {
        public override void OnOpen() {
            base.OnOpen();

            if (NodeEditorWindow.current != null) {
                NodeEditorWindow.current.titleContent =
                    new GUIContent($"Dungeon – {target.name}");
            }
        }

        public override Color GetPortColor(NodePort port) {
            if (!ColorForType(port.ValueType, out Color color)) {
                color = base.GetPortColor(port);
            }
            return color;
        }

        private bool ColorForType(System.Type type, out Color color) {
            color = default;
            if (type == typeof(DungeonConnection)) {
                color = new Color(1f, 0.8f, 0.2f);
                return true;
            } else if (type == typeof(SourceConnection)) {
                color = new Color(0.2f, 1f, 0.2f);
                return true;
            } else if (type == typeof(BoundsConnection)) {
                color = new Color(1f, 0.5f, 0.0f);
                return true;
            } else if (type == typeof(NoiseConnection)) {
                color = new Color(0.0f, 0.8f, 1f);
                return true;
            } else if (type == typeof(MatcherConnection)) {
                color = new Color(1f, 0.4f, 0.8f);
                return true;
            } else if (type == typeof(EntitySourceConnection)) {
                color = new Color(0.6f, 0.4f, 1f);
                return true;
            }
            return false;
        }
    }
}