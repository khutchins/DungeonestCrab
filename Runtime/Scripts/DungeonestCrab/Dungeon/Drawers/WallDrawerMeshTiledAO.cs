using DungeonestCrab.Dungeon.Printer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Wall - Mesh Tiled")]
    public class WallDrawerMeshTiledAO : IWallDrawer {
        [InlineEditor][SerializeField] TextureView Texture;
        [Tooltip("If true, will be aligned relative to world Y, not 0.")]
        [SerializeField] bool OffsetTextureByPosition = false;

        [Header("AO Settings")]
        [Tooltip("Width of the AO falloff band, measured in source-texture pixels.")]
        [Range(0, 16)][SerializeField] int RampWidthPixels = 3;
        [Tooltip("Color the AO band darkens toward at the wall edge. Alpha controls AO strength (0 = no AO, 1 = full).")]
        [SerializeField] Color AOColor = Color.black;
        [Tooltip("Quantize the AO band to discrete per-pixel levels.")]
        [SerializeField] bool PixelAlignedRamp = false;

        public override void DrawWall(WallInfo info) {
            if (Texture == null || Texture.Material == null) return;

            GameObject wallObj = new GameObject($"TiledWall");
            wallObj.transform.SetParent(info.parent, false);

            Quaternion angle = Quaternion.AngleAxis(info.rotation, Vector3.up);
            Vector3 upDir = angle * Vector3.up;
            Vector3 rightDir = angle * Vector3.right;
            Vector3 forwardDir = angle * Vector3.forward;

            float width = info.tileSize.x;
            Vector3 faceCenter = info.position + forwardDir * (width * 0.5f);
            Vector3 bottomLeftOrigin = faceCenter - (rightDir * width * 0.5f);

            Mesher mesher = new Mesher(wallObj, false);

            float tileHeight = info.tileSize.y;
            float currentY = info.minY;

            bool wallLeft = GetIsAdjacent(info.wallDraws, WallAdjacency.BottomLeft);
            bool wallRight = GetIsAdjacent(info.wallDraws, WallAdjacency.BottomRight);
            bool wallTop = true; // Walls generally meet ceilings
            bool wallBottom = true; // Walls generally meet floors

            Vector2Int texSize = SafeTextureSize(Texture);
            float rampX = Mathf.Min(width * 0.499f, width * RampWidthPixels / texSize.x);
            float rampY = Mathf.Min(tileHeight * 0.499f, tileHeight * RampWidthPixels / texSize.y);

            float texYOffset = OffsetTextureByPosition ? (info.position.y % tileHeight) : 0;

            bool aoActive = (wallLeft || wallRight || wallTop || wallBottom) && RampWidthPixels > 0 && texSize.x > 0 && texSize.y > 0;
            
            int rx = Mathf.Min(RampWidthPixels, (texSize.x - 1) / 2);
            int ry = Mathf.Min(RampWidthPixels, (texSize.y - 1) / 2);
            bool usePixelAligned = PixelAlignedRamp && aoActive && rx >= 1 && ry >= 1;

            void EmitRegion(float x0, float y0, float x1, float y1, int subX, int subY, bool hasAO, System.Func<int, int, Color> colorFn) {
                if (x1 <= x0 || y1 <= y0) return;

                if (!hasAO) {
                    float uV_L = x0 / width;
                    float uV_R = x1 / width;
                    float uV_B = 1f - (y0 / tileHeight);
                    float uV_T = 1f - (y1 / tileHeight);
                    mesher.GenerateRect(Texture.Material,
                        mesher.AddVert(Texture.Material, bottomLeftOrigin + rightDir * x0 + upDir * (currentY + y0), Texture.Lerp(uV_L, uV_B)),
                        mesher.AddVert(Texture.Material, bottomLeftOrigin + rightDir * x1 + upDir * (currentY + y0), Texture.Lerp(uV_R, uV_B)),
                        mesher.AddVert(Texture.Material, bottomLeftOrigin + rightDir * x0 + upDir * (currentY + y1), Texture.Lerp(uV_L, uV_T)),
                        mesher.AddVert(Texture.Material, bottomLeftOrigin + rightDir * x1 + upDir * (currentY + y1), Texture.Lerp(uV_R, uV_T))
                    );
                    return;
                }
                
                float dx = (x1 - x0) / subX;
                float dy = (y1 - y0) / subY;
                for (int i = 0; i < subX; i++) {
                    for (int j = 0; j < subY; j++) {
                        float xa = x0 + i * dx;
                        float xb = x0 + (i + 1) * dx;
                        float ya = y0 + j * dy;
                        float yb = y0 + (j + 1) * dy;
                        Color c = colorFn(i, j);

                        float uV_L = xa / width;
                        float uV_R = xb / width;
                        float uV_B = 1f - (ya / tileHeight);
                        float uV_T = 1f - (yb / tileHeight);

                        mesher.GenerateRect(Texture.Material,
                            mesher.AddVert(Texture.Material, bottomLeftOrigin + rightDir * xa + upDir * (currentY + ya), Texture.Lerp(uV_L, uV_B), c),
                            mesher.AddVert(Texture.Material, bottomLeftOrigin + rightDir * xb + upDir * (currentY + ya), Texture.Lerp(uV_R, uV_B), c),
                            mesher.AddVert(Texture.Material, bottomLeftOrigin + rightDir * xa + upDir * (currentY + yb), Texture.Lerp(uV_L, uV_T), c),
                            mesher.AddVert(Texture.Material, bottomLeftOrigin + rightDir * xb + upDir * (currentY + yb), Texture.Lerp(uV_R, uV_T), c)
                        );
                    }
                }
            }

            while (currentY < info.maxY) {
                float nextY = Mathf.Min(currentY + tileHeight, info.maxY);
                float segmentHeight = nextY - currentY;
                
                bool isTop = nextY >= info.maxY;
                bool isBottom = currentY <= info.minY;

                if (usePixelAligned) {
                    float rY_bot = isBottom ? rampY : 0;
                    float rY_top = isTop ? rampY : 0;
                    float rX_left = wallLeft ? rampX : 0;
                    float rX_right = wallRight ? rampX : 0;
                    
                    int tx = texSize.x;
                    int ty = texSize.y;

                    Color PixColor(int px, int py) {
                        float dL = wallLeft && px < rx ? (float)(RampWidthPixels - px) / RampWidthPixels : 0f;
                        float dR = wallRight && tx - 1 - px < rx ? (float)(RampWidthPixels - (tx - 1 - px)) / RampWidthPixels : 0f;
                        float dB = isBottom && py < ry ? (float)(RampWidthPixels - py) / RampWidthPixels : 0f;
                        float dT = isTop && ty - 1 - py < ry ? (float)(RampWidthPixels - (ty - 1 - py)) / RampWidthPixels : 0f;

                        float darkness = Mathf.Max(Mathf.Max(dL, dR), Mathf.Max(dB, dT)) * AOColor.a;
                        return new Color(
                            Mathf.Lerp(1f, AOColor.r, darkness),
                            Mathf.Lerp(1f, AOColor.g, darkness),
                            Mathf.Lerp(1f, AOColor.b, darkness),
                            1f);
                    }

                    float x1 = rX_left;
                    float x2 = width - rX_right;
                    float y1 = rY_bot;
                    float y2 = segmentHeight - rY_top;

                    // 9 Regions
                    // Bottom
                    EmitRegion(0, 0, x1, y1, rx, ry, wallBottom || wallLeft, (i, j) => PixColor(i, j));
                    EmitRegion(x1, 0, x2, y1, 1, ry, wallBottom, (i, j) => PixColor(rx, j));
                    EmitRegion(x2, 0, width, y1, rx, ry, wallBottom || wallRight, (i, j) => PixColor(tx - rx + i, j));
                    
                    // Middle
                    EmitRegion(0, y1, x1, y2, rx, 1, wallLeft, (i, j) => PixColor(i, ry));
                    EmitRegion(x1, y1, x2, y2, 1, 1, false, (i, j) => Color.white);
                    EmitRegion(x2, y1, width, y2, rx, 1, wallRight, (i, j) => PixColor(tx - rx + i, ry));
                    
                    // Top
                    EmitRegion(0, y2, x1, segmentHeight, rx, ry, wallTop || wallLeft, (i, j) => PixColor(i, ty - ry + j));
                    EmitRegion(x1, y2, x2, segmentHeight, 1, ry, wallTop, (i, j) => PixColor(rx, ty - ry + j));
                    EmitRegion(x2, y2, width, segmentHeight, rx, ry, wallTop || wallRight, (i, j) => PixColor(tx - rx + i, ty - ry + j));

                } else if (aoActive) {
                    float rY_bot = isBottom ? rampY : 0;
                    float rY_top = isTop ? rampY : 0;
                    float rX_left = wallLeft ? rampX : 0;
                    float rX_right = wallRight ? rampX : 0;

                    float[] xs = { 0, rX_left, width - rX_right, width };
                    float[] ys = { 0, rY_bot, segmentHeight - rY_top, segmentHeight };

                    int[,] vIdx = new int[4, 4];
                    for (int xi = 0; xi < 4; xi++) {
                        for (int yi = 0; yi < 4; yi++) {
                            float lx = xs[xi];
                            float ly = ys[yi];

                            Vector3 pos = bottomLeftOrigin + (rightDir * lx) + (upDir * (currentY + ly));
                            
                            float uvX = lx / width;
                            float uvY = 1f - (ly / tileHeight);
                            Vector2 uv = Texture.Lerp(uvX, uvY);

                            float dL = wallLeft && rampX > 0 ? Mathf.Max(0, 1f - lx / rampX) : 0;
                            float dR = wallRight && rampX > 0 ? Mathf.Max(0, 1f - (width - lx) / rampX) : 0;
                            float dB = isBottom && rampY > 0 ? Mathf.Max(0, 1f - ly / rampY) : 0;
                            float dT = isTop && rampY > 0 ? Mathf.Max(0, 1f - (segmentHeight - ly) / rampY) : 0;

                            float darkness = Mathf.Max(Mathf.Max(dL, dR), Mathf.Max(dB, dT)) * AOColor.a;
                            Color c = new Color(
                                Mathf.Lerp(1, AOColor.r, darkness),
                                Mathf.Lerp(1, AOColor.g, darkness),
                                Mathf.Lerp(1, AOColor.b, darkness),
                                1);

                            vIdx[xi, yi] = mesher.AddVert(Texture.Material, pos, uv, c);
                        }
                    }

                    for (int xi = 0; xi < 3; xi++) {
                        for (int yi = 0; yi < 3; yi++) {
                            if (xs[xi] == xs[xi + 1] || ys[yi] == ys[yi + 1]) continue;

                            int bl = vIdx[xi, yi];
                            int br = vIdx[xi + 1, yi];
                            int tl = vIdx[xi, yi + 1];
                            int tr = vIdx[xi + 1, yi + 1];

                            bool altDiagonal = (xi == 0 && yi == 0) || (xi == 2 && yi == 2);
                            if (altDiagonal) {
                                mesher.AddTriangle(Texture.Material, bl, tr, br);
                                mesher.AddTriangle(Texture.Material, bl, tl, tr);
                            } else {
                                mesher.GenerateRect(Texture.Material, bl, br, tl, tr);
                            }
                        }
                    }
                } else {
                    // Fallback to simple quad
                    Vector3 vBottomLeft = bottomLeftOrigin + (upDir * currentY);
                    Vector3 vBottomRight = vBottomLeft + (rightDir * width);
                    Vector3 vTopLeft = vBottomLeft + (upDir * segmentHeight);
                    Vector3 vTopRight = vBottomRight + (upDir * segmentHeight);

                    float uvHeightPercent = segmentHeight / tileHeight;
                    Vector2 uvBL = Texture.Lerp(0, 1);
                    Vector2 uvBR = Texture.Lerp(1, 1);
                    Vector2 uvTL = Texture.Lerp(0, 1 - uvHeightPercent);
                    Vector2 uvTR = Texture.Lerp(1, 1 - uvHeightPercent);

                    mesher.GenerateRect(Texture.Material,
                        mesher.AddVert(Texture.Material, vBottomLeft, uvBL),
                        mesher.AddVert(Texture.Material, vBottomRight, uvBR),
                        mesher.AddVert(Texture.Material, vTopLeft, uvTL),
                        mesher.AddVert(Texture.Material, vTopRight, uvTR)
                    );
                }

                currentY = nextY;
            }

            mesher.Finish();
        }

        static Vector2Int SafeTextureSize(TextureView tv) {
            if (tv == null || tv.Material == null) return Vector2Int.one;
            if (tv.Material.GetTexture("_MainTex") == null) return Vector2Int.one;
            return tv.TextureSize;
        }
    }
}