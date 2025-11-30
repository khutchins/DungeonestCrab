using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    /// <summary>
    /// A node that takes a dungeon, modifies it, and passes it on.
    /// </summary>
    public abstract class DungeonPassNode : DungeonNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public DungeonConnection Input;
        [Output(ShowBackingValue.Never, ConnectionType.Override)] public DungeonConnection Output;

        // Editor Logic
        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return GetPreviewDungeon();
            return null;
        }

        public override TheDungeon GetPreviewDungeon() {
            TheDungeon inputDungeon = GetInputValue<TheDungeon>("Input", null);

            // Clone for preview snapshot isolation
            TheDungeon workingDungeon = inputDungeon != null ? inputDungeon.Clone() : null;

            if (workingDungeon != null) {
                ApplyNodeLogic(workingDungeon, new SystemRandom(12345));
                UpdateTexture(workingDungeon);
            } else {
                PreviewTexture = null;
            }

            _cachedPreview = workingDungeon;
            return _cachedPreview;
        }

        // Runtime Logic
        public override TheDungeon GenerateRuntime(IRandom random, TheDungeon incomingDungeon) {
            if (incomingDungeon == null) {
                Debug.LogError($"Node {name} received null dungeon at runtime.");
                return null;
            }

            ApplyNodeLogic(incomingDungeon, random);

            NodePort outPort = GetOutputPort("Output");
            if (outPort.IsConnected) {
                DungeonNode nextNode = outPort.Connection.node as DungeonNode;
                if (nextNode != null) {
                    return nextNode.GenerateRuntime(random, incomingDungeon);
                }
            }

            return incomingDungeon;
        }
    }
}