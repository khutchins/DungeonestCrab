using DungeonestCrab.Dungeon;
using DungeonestCrab.Dungeon.Printer;
using Pomerandomian;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Start")]
    public class StartNode : DungeonNode {
        [Output(ShowBackingValue.Never, ConnectionType.Override)] public DungeonConnection Output;

        [Header("Settings")]
        public List<DungeonTraitSO> Traits;

        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return GetPreviewDungeon();
            return null;
        }

        public override TheDungeon GetPreviewDungeon() {
            Vector2Int size = GetDimensions();
            TheDungeon dungeon = new TheDungeon(size.x, size.y, new SystemRandom(12345));
            if (Traits != null) dungeon.Traits.AddRange(Traits);
            UpdateTexture(dungeon);
            _cachedPreview = dungeon;
            return dungeon;
        }

        public override bool GenerateRuntime(IRandom random, TheDungeon incomingDungeon) {
            if (Traits != null) incomingDungeon.Traits.AddRange(Traits);

            return base.GenerateRuntime(random, incomingDungeon);
        }

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            return true;
        }
    }
}