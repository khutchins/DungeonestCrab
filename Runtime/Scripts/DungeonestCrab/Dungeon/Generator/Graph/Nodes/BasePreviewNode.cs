using UnityEngine;
using XNode;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    public abstract class BasePreviewNode : Node {
        public enum PreviewMode {
            Geometry,
            Terrain,
            Style,
            Regions
        }

        [HideInInspector] public bool ShowPreview = true;
        [HideInInspector] public Texture2D PreviewTexture;
        [HideInInspector] public PreviewMode ViewMode = PreviewMode.Geometry;

        public abstract void UpdatePreview();

        protected void ValidatePreviewTexture(int width, int height) {
            if (PreviewTexture == null || PreviewTexture.width != width || PreviewTexture.height != height) {
                if (PreviewTexture != null) DestroyImmediate(PreviewTexture);
                PreviewTexture = new Texture2D(width, height) {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };
            }
        }

        public Vector2Int GetDimensions() {
            if (graph is DungeonGraph gg) {
                return new Vector2Int(gg.Width, gg.Height);
            }
            // If not in the graph.
            return new Vector2Int(40, 40);
        }

        protected int GetPreviewSeed() {
            if (graph is DungeonGraph dg) {
                return dg.DebugSeed + this.GetInstanceID();
            }
            return 0;
        }
    }
}