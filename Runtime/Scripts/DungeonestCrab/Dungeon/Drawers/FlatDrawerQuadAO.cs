using Sirenix.OdinInspector;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Floor - Quad AO")]
    public class FlatDrawerQuadAO : FlatDrawerMesh {
        [InlineEditor][SerializeField] TextureView FloorTexture;
        [Tooltip("Width of the AO falloff band, measured in source-texture pixels.")]
        [Range(0, 16)][SerializeField] int RampWidthPixels = 3;
        [Tooltip("Color the AO band darkens toward at the wall edge. Alpha controls AO strength (0 = no AO, 1 = full).")]
        [SerializeField] Color AOColor = Color.black;
        [Tooltip("Quantize the AO band to discrete per-pixel levels.")]
        [SerializeField] bool PixelAlignedRamp = false;

        public override bool DrawAtCenter { get => false; }

        public override void Draw(FlatInfo info, Mesher mesher) {
            if (FloorTexture == null || FloorTexture.Material == null) return;

            Material mat = FloorTexture.Material;
            float sx = info.tileSize.x;
            float sz = info.tileSize.z;
            Vector2Int texSize = SafeTextureSize(FloorTexture);
            int rampPixels = Mathf.Max(0, RampWidthPixels);

            bool wallN = !info.tileSpec.AreTileTypesTheSameInDirections(TileSpec.Adjacency.N);
            bool wallE = !info.tileSpec.AreTileTypesTheSameInDirections(TileSpec.Adjacency.E);
            bool wallS = !info.tileSpec.AreTileTypesTheSameInDirections(TileSpec.Adjacency.S);
            bool wallW = !info.tileSpec.AreTileTypesTheSameInDirections(TileSpec.Adjacency.W);
            bool wallNE = !info.tileSpec.AreTileTypesTheSameInDirections(TileSpec.Adjacency.NE);
            bool wallSE = !info.tileSpec.AreTileTypesTheSameInDirections(TileSpec.Adjacency.SE);
            bool wallSW = !info.tileSpec.AreTileTypesTheSameInDirections(TileSpec.Adjacency.SW);
            bool wallNW = !info.tileSpec.AreTileTypesTheSameInDirections(TileSpec.Adjacency.NW);
            bool anyWall = wallN || wallE || wallS || wallW || wallNE || wallSE || wallSW || wallNW;

            bool aoActive = anyWall && rampPixels > 0 && texSize.x > 0 && texSize.y > 0;
            if (!aoActive) {
                EmitFlatQuad(mesher, mat, 0, 0, sx, sz, sx, sz);
                return;
            }

            if (PixelAlignedRamp) {
                DrawPixelAligned(mesher, mat, sx, sz, texSize, rampPixels,
                                 wallN, wallE, wallS, wallW,
                                 wallNE, wallSE, wallSW, wallNW);
            } else {
                float rampX = Mathf.Min(sx * 0.499f, sx * rampPixels / texSize.y);
                float rampZ = Mathf.Min(sz * 0.499f, sz * rampPixels / texSize.x);
                DrawNinepatch(mesher, mat, sx, sz, rampX, rampZ,
                              wallN, wallE, wallS, wallW,
                              wallNE, wallSE, wallSW, wallNW);
            }
        }

        void DrawNinepatch(Mesher mesher, Material mat, float sx, float sz, float rampX, float rampZ,
                           bool wallN, bool wallE, bool wallS, bool wallW,
                           bool wallNE, bool wallSE, bool wallSW, bool wallNW) {
            float[] xs = { 0f, rampX, sx - rampX, sx };
            float[] zs = { 0f, rampZ, sz - rampZ, sz };
            int[,] vIdx = new int[4, 4];

            for (int xi = 0; xi < 4; xi++) {
                for (int zi = 0; zi < 4; zi++) {
                    float x = xs[xi];
                    float z = zs[zi];
                    Color c = SmoothColor(x, z, sx, sz, rampX, rampZ,
                                          wallN, wallE, wallS, wallW,
                                          wallNE, wallSE, wallSW, wallNW);
                    Vector2 uv = FloorTexture.Lerp(z / sz, x / sx);
                    vIdx[xi, zi] = mesher.AddVert(mat, new Vector3(x, 0, z), uv, c);
                }
            }

            for (int xi = 0; xi < 3; xi++) {
                for (int zi = 0; zi < 3; zi++) {
                    int bl = vIdx[xi, zi];
                    int br = vIdx[xi + 1, zi];
                    int tl = vIdx[xi, zi + 1];
                    int tr = vIdx[xi + 1, zi + 1];

                    bool altDiagonal = (xi == 0 && zi == 0) || (xi == 2 && zi == 2);
                    if (altDiagonal) {
                        mesher.AddTriangle(mat, bl, tr, br);
                        mesher.AddTriangle(mat, bl, tl, tr);
                    } else {
                        mesher.GenerateRect(mat, bl, br, tl, tr);
                    }
                }
            }
        }

        void DrawPixelAligned(Mesher mesher, Material mat, float sx, float sz, Vector2Int texSize,
                              int rampPixels, bool wallN, bool wallE, bool wallS, bool wallW,
                              bool wallNE, bool wallSE, bool wallSW, bool wallNW) {
            int rx = Mathf.Min(rampPixels, (texSize.y - 1) / 2);
            int rz = Mathf.Min(rampPixels, (texSize.x - 1) / 2);
            if (rx < 1 || rz < 1) {
                float fbX = Mathf.Min(sx * 0.499f, sx * rampPixels / texSize.y);
                float fbZ = Mathf.Min(sz * 0.499f, sz * rampPixels / texSize.x);
                DrawNinepatch(mesher, mat, sx, sz, fbX, fbZ,
                              wallN, wallE, wallS, wallW,
                              wallNE, wallSE, wallSW, wallNW);
                return;
            }

            float dx = sx / texSize.y;
            float dz = sz / texSize.x;
            float x1 = rx * dx;
            float x2 = sx - rx * dx;
            float z1 = rz * dz;
            float z2 = sz - rz * dz;
            int tx = texSize.y;
            int ty = texSize.x;

            Color PixColor(int px, int py) {
                float dW = wallW && px < rx ? (float)(rampPixels - px) / rampPixels : 0f;
                float dE = wallE && tx - 1 - px < rx ? (float)(rampPixels - (tx - 1 - px)) / rampPixels : 0f;
                float dS = wallS && py < rz ? (float)(rampPixels - py) / rampPixels : 0f;
                float dN = wallN && ty - 1 - py < rz ? (float)(rampPixels - (ty - 1 - py)) / rampPixels : 0f;
                
                float dSW = wallSW && px < rx && py < rz ? (float)(rampPixels - (px + py)) / rampPixels : 0f;
                float dSE = wallSE && tx - 1 - px < rx && py < rz ? (float)(rampPixels - ((tx - 1 - px) + py)) / rampPixels : 0f;
                float dNW = wallNW && px < rx && ty - 1 - py < rz ? (float)(rampPixels - (px + (ty - 1 - py))) / rampPixels : 0f;
                float dNE = wallNE && tx - 1 - px < rx && ty - 1 - py < rz ? (float)(rampPixels - ((tx - 1 - px) + (ty - 1 - py))) / rampPixels : 0f;

                float darkness = Mathf.Max(Mathf.Max(dN, dS), Mathf.Max(dE, dW));
                darkness = Mathf.Max(darkness, Mathf.Max(Mathf.Max(dNE, dNW), Mathf.Max(dSE, dSW))) * AOColor.a;

                return new Color(
                    Mathf.Lerp(1f, AOColor.r, darkness),
                    Mathf.Lerp(1f, AOColor.g, darkness),
                    Mathf.Lerp(1f, AOColor.b, darkness),
                    1f);
            }

            EmitRegion(mesher, mat, 0, 0, x1, z1, rx, rz, wallS || wallW || wallSW, sx, sz,
                       (i, j) => PixColor(i, j));
            EmitRegion(mesher, mat, x1, 0, x2, z1, 1, rz, wallS, sx, sz,
                       (i, j) => PixColor(rx, j));
            EmitRegion(mesher, mat, x2, 0, sx, z1, rx, rz, wallS || wallE || wallSE, sx, sz,
                       (i, j) => PixColor(tx - rx + i, j));

            EmitRegion(mesher, mat, 0, z1, x1, z2, rx, 1, wallW, sx, sz,
                       (i, j) => PixColor(i, rz));
            EmitFlatQuad(mesher, mat, x1, z1, x2, z2, sx, sz);
            EmitRegion(mesher, mat, x2, z1, sx, z2, rx, 1, wallE, sx, sz,
                       (i, j) => PixColor(tx - rx + i, rz));

            EmitRegion(mesher, mat, 0, z2, x1, sz, rx, rz, wallN || wallW || wallNW, sx, sz,
                       (i, j) => PixColor(i, ty - rz + j));
            EmitRegion(mesher, mat, x1, z2, x2, sz, 1, rz, wallN, sx, sz,
                       (i, j) => PixColor(rx, ty - rz + j));
            EmitRegion(mesher, mat, x2, z2, sx, sz, rx, rz, wallN || wallE || wallNE, sx, sz,
                       (i, j) => PixColor(tx - rx + i, ty - rz + j));
        }

        void EmitRegion(Mesher mesher, Material mat,
                        float x0, float z0, float x1, float z1,
                        int subX, int subZ, bool hasAO, float sx, float sz,
                        System.Func<int, int, Color> colorFn) {
            if (!hasAO) {
                EmitFlatQuad(mesher, mat, x0, z0, x1, z1, sx, sz);
                return;
            }
            float dx = (x1 - x0) / subX;
            float dz = (z1 - z0) / subZ;
            for (int i = 0; i < subX; i++) {
                for (int j = 0; j < subZ; j++) {
                    float xa = x0 + i * dx;
                    float xb = x0 + (i + 1) * dx;
                    float za = z0 + j * dz;
                    float zb = z0 + (j + 1) * dz;
                    Color c = colorFn(i, j);
                    mesher.GenerateRect(mat,
                        mesher.AddVert(mat, new Vector3(xa, 0, za), FloorTexture.Lerp(za / sz, xa / sx), c),
                        mesher.AddVert(mat, new Vector3(xb, 0, za), FloorTexture.Lerp(za / sz, xb / sx), c),
                        mesher.AddVert(mat, new Vector3(xa, 0, zb), FloorTexture.Lerp(zb / sz, xa / sx), c),
                        mesher.AddVert(mat, new Vector3(xb, 0, zb), FloorTexture.Lerp(zb / sz, xb / sx), c)
                    );
                }
            }
        }

        void EmitFlatQuad(Mesher mesher, Material mat, float x0, float z0, float x1, float z1, float sx, float sz) {
            mesher.GenerateRect(mat,
                mesher.AddVert(mat, new Vector3(x0, 0, z0), FloorTexture.Lerp(z0 / sz, x0 / sx)),
                mesher.AddVert(mat, new Vector3(x1, 0, z0), FloorTexture.Lerp(z0 / sz, x1 / sx)),
                mesher.AddVert(mat, new Vector3(x0, 0, z1), FloorTexture.Lerp(z1 / sz, x0 / sx)),
                mesher.AddVert(mat, new Vector3(x1, 0, z1), FloorTexture.Lerp(z1 / sz, x1 / sx))
            );
        }

        Color SmoothColor(float x, float z, float sx, float sz, float rampX, float rampZ,
                          bool wallN, bool wallE, bool wallS, bool wallW,
                          bool wallNE, bool wallSE, bool wallSW, bool wallNW) {
            float dN = wallN ? Mathf.Max(0f, 1f - (sz - z) / rampZ) : 0f;
            float dS = wallS ? Mathf.Max(0f, 1f - z / rampZ) : 0f;
            float dE = wallE ? Mathf.Max(0f, 1f - (sx - x) / rampX) : 0f;
            float dW = wallW ? Mathf.Max(0f, 1f - x / rampX) : 0f;

            float dNE = wallNE ? Mathf.Max(0f, 1f - ((sx - x) / rampX + (sz - z) / rampZ)) : 0f;
            float dSE = wallSE ? Mathf.Max(0f, 1f - ((sx - x) / rampX + z / rampZ)) : 0f;
            float dSW = wallSW ? Mathf.Max(0f, 1f - (x / rampX + z / rampZ)) : 0f;
            float dNW = wallNW ? Mathf.Max(0f, 1f - (x / rampX + (sz - z) / rampZ)) : 0f;

            float darkness = Mathf.Max(Mathf.Max(dN, dS), Mathf.Max(dE, dW));
            darkness = Mathf.Max(darkness, Mathf.Max(Mathf.Max(dNE, dNW), Mathf.Max(dSE, dSW))) * AOColor.a;
            return new Color(
                Mathf.Lerp(1f, AOColor.r, darkness),
                Mathf.Lerp(1f, AOColor.g, darkness),
                Mathf.Lerp(1f, AOColor.b, darkness),
                1f);
        }

        static Vector2Int SafeTextureSize(TextureView tv) {
            if (tv == null || tv.Material == null) return Vector2Int.zero;
            if (tv.Material.GetTexture("_MainTex") == null) return Vector2Int.zero;
            return tv.TextureSize;
        }
    }
}
