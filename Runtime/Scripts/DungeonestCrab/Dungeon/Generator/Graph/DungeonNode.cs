using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    public abstract class DungeonNode : BasePreviewNode {
        protected TheDungeon _cachedPreview;

        public void UpdateTexture(TheDungeon dungeon) {
            if (dungeon == null) {
                return;
            }

            ValidatePreviewTexture(dungeon.Size.x, dungeon.Size.y);

            Color[] colors = new Color[dungeon.Size.x * dungeon.Size.y];

            int[,] regions = null;
            int maxRegion = 0;
            if (ViewMode == PreviewMode.Regions) {
                regions = dungeon.ComputeRegions(out maxRegion);
            }

            for (int y = 0; y < dungeon.Size.y; y++) {
                for (int x = 0; x < dungeon.Size.x; x++) {
                    TileSpec tile = dungeon.GetTileSpecSafe(x, y);
                    int i = y * dungeon.Size.x + x;

                    switch (ViewMode) {
                        case PreviewMode.Geometry:
                            colors[i] = GetGeometryColor(tile);
                            break;
                        case PreviewMode.Terrain:
                            colors[i] = GetHashColor(tile.Terrain);
                            break;
                        case PreviewMode.Style:
                            colors[i] = GetStyleColor(tile.Style);
                            break;
                        case PreviewMode.Regions:
                            colors[i] = GetRegionColor(regions[y, x]);
                            break;
                    }
                }
            }

            if (ViewMode == PreviewMode.Geometry) {
                foreach (var ent in dungeon.Entities) {
                    if (dungeon.Contains(ent.Tile)) {
                        int i = ent.Tile.y * dungeon.Size.x + ent.Tile.x;
                        if (i >= 0 && i < colors.Length) colors[i] = Color.red;
                    }
                }
            }

            PreviewTexture.SetPixels(colors);
            PreviewTexture.Apply();
        }

        private Color GetGeometryColor(TileSpec tile) {
            if (tile.Tile == Tile.Wall) return Color.black;
            if (tile.Tile == Tile.Floor) return Color.white;
            return new Color(0.25f, 0.25f, 0.25f); // Unset
        }

        private Color GetStyleColor(int styleId) {
            if (styleId <= 0) return new Color(0.2f, 0.2f, 0.2f);
            return GetHashColor(styleId);
        }

        private Color GetRegionColor(int regionId) {
            if (regionId == -1) return Color.black;
            return GetHashColor(regionId);
        }

        private Color GetHashColor(object key) {
            if (key == null) return new Color(0.25f, 0.25f, 0.25f);
            int hash = key.GetHashCode();
            System.Random r = new System.Random(hash);
            return Color.HSVToRGB((float)r.NextDouble(), 0.7f, 0.9f);
        }

        public override void UpdatePreview() {
            GetPreviewDungeon();
        }

        public abstract TheDungeon GetPreviewDungeon();

        public virtual bool GenerateRuntime(IRandom random, TheDungeon dungeon) {
            if (!ApplyNodeLogic(dungeon, random)) {
                Debug.LogWarning($"Generation failed at node: {name}");
                return false;
            }

            NodePort outPort = GetOutputPort("Output");
            if (outPort != null && outPort.IsConnected) {
                DungeonNode nextNode = outPort.Connection.node as DungeonNode;
                if (nextNode != null) {
                    return nextNode.GenerateRuntime(random, dungeon);
                }
            }

            return true;
        }

        protected abstract bool ApplyNodeLogic(TheDungeon dungeon, IRandom random);
    }
}