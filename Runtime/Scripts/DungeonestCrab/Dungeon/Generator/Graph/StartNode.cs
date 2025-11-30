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

        public override bool GenerateRuntime(IRandom random, TheDungeon incomingDungeon) {
            incomingDungeon.Trait = DungeonTrait;

            return base.GenerateRuntime(random, incomingDungeon);
        }

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            return true;
        }
    }
}