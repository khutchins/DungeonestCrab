using DungeonestCrab.Dungeon.Printer;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Wall - Mesh Tiled")]
    public class WallDrawerMeshTiled : IWallDrawer {
        [SerializeField] TextureView Texture;
        [SerializeField] bool OffsetTextureByPosition = false;

        public override void DrawWall(WallInfo info) {
            // 1. Create Container
            GameObject wallObj = new GameObject($"TiledWall");
            wallObj.transform.SetParent(info.parent, false);

            Quaternion angle = Quaternion.AngleAxis(info.rotation, Vector3.up);
            Vector3 upDir = angle * Vector3.up;
            Vector3 rightDir = angle * Vector3.right;
            Vector3 forwardDir = angle * Vector3.forward;

            float width = info.tileSize.x;
            Vector3 faceCenter = info.position;
            Vector3 bottomLeftOrigin = faceCenter - (rightDir * width * 0.5f);

            Mesher mesher = new Mesher(wallObj, false);

            float tileHeight = info.tileSize.y;
            float currentY = info.minY;

            float yOffset = OffsetTextureByPosition ? (info.position.y % tileHeight) : 0;

            while (currentY < info.maxY) {
                float nextY = Mathf.Min(currentY + tileHeight, info.maxY);
                float segmentHeight = nextY - currentY;

                float uvHeightPercent = segmentHeight / tileHeight;

                Vector3 vBottomLeft = bottomLeftOrigin + (upDir * currentY);
                Vector3 vBottomRight = vBottomLeft + (rightDir * width);
                Vector3 vTopLeft = vBottomLeft + (upDir * segmentHeight);
                Vector3 vTopRight = vBottomRight + (upDir * segmentHeight);

                Vector2 uvBL = Texture.Lerp(0, 0);
                Vector2 uvBR = Texture.Lerp(1, 0);
                Vector2 uvTL = Texture.Lerp(0, uvHeightPercent);
                Vector2 uvTR = Texture.Lerp(1, uvHeightPercent);

                mesher.GenerateRect(Texture.Material,
                    mesher.AddVert(Texture.Material, vBottomLeft, uvBL),
                    mesher.AddVert(Texture.Material, vBottomRight, uvBR),
                    mesher.AddVert(Texture.Material, vTopLeft, uvTL),
                    mesher.AddVert(Texture.Material, vTopRight, uvTR)
                );

                currentY = nextY;
            }

            mesher.Finish();
        }
    }
}