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
        public Trait DungeonTrait = Trait.None;

        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return GetPreviewDungeon();
            return null;
        }

        public override TheDungeon GetPreviewDungeon() {
            Vector2Int size = GetDimensions();
            TheDungeon d = new TheDungeon(size.x, size.y, new SystemRandom(12345));
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