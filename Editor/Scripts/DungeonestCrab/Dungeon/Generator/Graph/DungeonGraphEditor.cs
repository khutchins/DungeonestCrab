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
    }
}