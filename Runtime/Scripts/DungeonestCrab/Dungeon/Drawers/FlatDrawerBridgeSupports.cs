using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Flat - Bridge Supports")]
    public class FlatDrawerBridgeSupports : FlatDrawerMesh {

        [InlineEditor]
        [SerializeField]
        private TextureView SupportTexture;

        [Header("Dimensions")]
        [Tooltip("Width of the pillar relative to tile size (0-0.5).")]
        [Range(0.05f, 0.4f)]
        [SerializeField] float SupportWidth = 0.05f;

        [Tooltip("How far to inset the pillar from the tile edge.")]
        [SerializeField] float Inset = 0f;

        [Tooltip("How far to raise past the bridge height.")]
        [SerializeField] float ExtraHeight = 0.4f;

        [Header("Logic")]
        [Tooltip("If true, different terrain doesn't count as adjacency.")]
        public bool RequireSameTerrain = true;

        private const int N = 1;
        private const int E = 2;
        private const int S = 4;
        private const int W = 8;

        public override void Draw(FlatInfo info, Mesher mesher) {
            int mask = 0;
            if (HasConnection(info, TileSpec.Adjacency.N)) mask |= N;
            if (HasConnection(info, TileSpec.Adjacency.E)) mask |= E;
            if (HasConnection(info, TileSpec.Adjacency.S)) mask |= S;
            if (HasConnection(info, TileSpec.Adjacency.W)) mask |= W;

            bool nw = false, ne = false, se = false, sw = false;

            switch (mask) {
                case N | S:
                case E | W:
                    break;

                case N | E:
                    ne = true; sw = true;
                    break;
                case S | E:
                    se = true; nw = true;
                    break;
                case S | W:
                    sw = true; ne = true;
                    break;
                case N | W:
                    nw = true; se = true;
                    break;

                case N | E | S | W:
                    nw = true; ne = true; se = true; sw = true;
                    break;

                case N | E | S:
                    ne = true; se = true;
                    break;
                case E | S | W:
                    se = true; sw = true;
                    break;
                case S | W | N:
                    sw = true; nw = true;
                    break;
                case W | N | E:
                    nw = true; ne = true;
                    break;

                case N:
                    se = true; sw = true;
                    break;
                case E:
                    nw = true; sw = true;
                    break;
                case S:
                    nw = true; ne = true;
                    break;
                case W:
                    ne = true; se = true;
                    break;
                default:
                    nw = true; ne = true; se = true; sw = true;
                    break;
            }

            if (!nw && !ne && !se && !sw) return;

            float height = info.tileSpec.GroundOffset + ExtraHeight;
            float yPos = -info.tileSpec.GroundOffset;

            float x0 = -info.tileSize.x / 2 + Inset;
            float x1 = info.tileSize.x / 2 - Inset;
            float z0 = -info.tileSize.z / 2 + Inset;
            float z1 = info.tileSize.z / 2 - Inset;

            float size = Mathf.Min(info.tileSize.x, info.tileSize.z) * SupportWidth;

            if (sw) DrawPillar(mesher, new Vector3(x0, yPos, z0), height, size);
            if (se) DrawPillar(mesher, new Vector3(x1, yPos, z0), height, size);
            if (ne) DrawPillar(mesher, new Vector3(x1, yPos, z1), height, size);
            if (nw) DrawPillar(mesher, new Vector3(x0, yPos, z1), height, size);
        }

        private void DrawPillar(Mesher mesher, Vector3 basePos, float height, float size) {
            mesher.GeneratePillar(SupportTexture.Material, basePos, size, height, SupportTexture.UV);

            Vector3 centerTop = basePos + new Vector3(0, height, 0);
            Vector3 forward = new Vector3(0, 0, size);
            Vector3 right = new Vector3(size, 0, 0);

            Vector3 bl = centerTop - right - forward;
            Vector3 br = centerTop + right - forward;
            Vector3 tl = centerTop - right + forward;
            Vector3 tr = centerTop + right + forward;

            Vector2[] capUVs = SupportTexture.UV;

            mesher.GenerateRect(SupportTexture.Material,
                mesher.AddVert(SupportTexture.Material, bl, capUVs[0]),
                mesher.AddVert(SupportTexture.Material, br, capUVs[3]),
                mesher.AddVert(SupportTexture.Material, tl, capUVs[1]),
                mesher.AddVert(SupportTexture.Material, tr, capUVs[2])
            );
        }

        private bool HasConnection(FlatInfo info, TileSpec.Adjacency dir) {
            if (RequireSameTerrain) {
                return info.tileSpec.AreTileAndTerrainTheSameInDirections(dir);
            } else {
                return info.tileSpec.AreTileTypesTheSameInDirections(dir);
            }
        }
    }
}