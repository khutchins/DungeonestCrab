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

            for (int y = 0; y < dungeon.Size.y; y++) {
                for (int x = 0; x < dungeon.Size.x; x++) {
                    Tile t = dungeon.GetTile(x, y);
                    int i = y * dungeon.Size.x + x;

                    if (t == Tile.Wall) colors[i] = Color.black;
                    else if (t == Tile.Floor) colors[i] = Color.white;
                    else colors[i] = new Color(0.25f, 0.25f, 0.25f);
                }
            }

            foreach (var ent in dungeon.Entities) {
                if (dungeon.Contains(ent.Tile)) {
                    int i = ent.Tile.y * dungeon.Size.x + ent.Tile.x;
                    if (i >= 0 && i < colors.Length) colors[i] = Color.red;
                }
            }

            PreviewTexture.SetPixels(colors);
            PreviewTexture.Apply();
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