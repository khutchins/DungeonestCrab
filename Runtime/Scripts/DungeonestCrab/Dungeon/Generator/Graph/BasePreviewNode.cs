using UnityEngine;
using XNode;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    public abstract class BasePreviewNode : Node {
        [HideInInspector] public bool ShowPreview = true;

        [HideInInspector] public Texture2D PreviewTexture;

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
    }
}