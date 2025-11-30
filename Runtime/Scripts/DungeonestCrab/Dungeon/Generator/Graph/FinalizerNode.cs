using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    [CreateNodeMenu("Dungeon/Finalizer")]
    public class FinalizerNode : DungeonNode {
        // Input Only
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public DungeonConnection Input;

        public TerrainSO DefaultTerrain;

        public override object GetValue(NodePort port) {
            return null; // Finalizer has no outputs
        }

        public override TheDungeon GetPreviewDungeon() {
            TheDungeon inputDungeon = GetInputValue<TheDungeon>("Input", null);
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

        public override TheDungeon GenerateRuntime(IRandom random, TheDungeon incomingDungeon) {
            if (incomingDungeon == null) return null;

            ApplyNodeLogic(incomingDungeon, random);

            // End of the line, return the result
            return incomingDungeon;
        }

        protected override void ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            if (DefaultTerrain == null) return;

            foreach (TileSpec tile in dungeon.AllTiles()) {
                // If the tile is being used (Wall or Floor) but has no specific terrain yet, apply default
                if (tile.Tile != Tile.Unset) {
                    tile.SetTerrainIfNull(DefaultTerrain);
                }
            }
        }
    }
}