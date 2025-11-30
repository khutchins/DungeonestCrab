using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Start")]
    public class StartNode : DungeonNode {
        // Output Only
        [Output(ShowBackingValue.Never, ConnectionType.Override)] public DungeonConnection Output;

        [Header("Settings")]
        public int Width = 40;
        public int Height = 40;
        public Trait DungeonTrait = Trait.None;

        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return GetPreviewDungeon();
            return null;
        }

        public override TheDungeon GetPreviewDungeon() {
            TheDungeon d = new TheDungeon(Width, Height, new SystemRandom(12345));
            d.Trait = DungeonTrait;
            UpdateTexture(d);
            _cachedPreview = d;
            return d;
        }

        public override TheDungeon GenerateRuntime(IRandom random, TheDungeon incomingDungeon = null) {
            // Start node IGNORES incomingDungeon and creates a new one
            TheDungeon newDungeon = new TheDungeon(Width, Height, random);
            newDungeon.Trait = DungeonTrait;

            NodePort outPort = GetOutputPort("Output");
            if (outPort.IsConnected) {
                DungeonNode nextNode = outPort.Connection.node as DungeonNode;
                if (nextNode != null) {
                    return nextNode.GenerateRuntime(random, newDungeon);
                }
            }
            return newDungeon;
        }

        protected override void ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            // No logic, purely structural
        }
    }
}