using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Finalizer")]
    public class FinalizerNode : DungeonNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public DungeonConnection Input;

        public TerrainSO DefaultTerrain;

        public override object GetValue(NodePort port) {
            return null;
        }

        public override TheDungeon GetPreviewDungeon() {
            TheDungeon inputDungeon = GetInputValue<TheDungeon>("Input", null);
            TheDungeon workingDungeon = inputDungeon != null ? inputDungeon.Clone() : null;

            if (workingDungeon != null) {
                ApplyNodeLogic(workingDungeon, new SystemRandom(GetPreviewSeed()));
                UpdateTexture(workingDungeon);
            } else {
                PreviewTexture = null;
            }

            _cachedPreview = workingDungeon;
            return _cachedPreview;
        }

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            return new Finalizer(DefaultTerrain).Modify(dungeon, random);
        }
    }
}