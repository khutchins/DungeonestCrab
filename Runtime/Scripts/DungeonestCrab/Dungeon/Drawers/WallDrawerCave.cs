using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Wall - Perturbed")]
    public class WallDrawerCave : IWallDrawer {
        [SerializeField] int VerticesPerSide = 5;
        [SerializeField] bool ConvergeAtEdges = false;
        [Range(0, 0.5f)][SerializeField] float MaxInset = 0.1f;
        [SerializeField] TextureView TextureView;

        public override void DrawWall(WallInfo info) {
            // This is the base.
            GameObject wall = new GameObject($"Wall ({info.tileSpec.Coords.x}, {info.tileSpec.Coords.y})");
            wall.transform.SetParent(info.parent);
            //wall.transform.localPosition = new Vector3(position.x, 0, position.z);
            //wall.transform.localEulerAngles = new Vector3(0, rot, 0);

            Quaternion angle = Quaternion.AngleAxis(info.rotation, Vector3.up);
            Vector3 up = angle * new Vector3(0, info.tileSize.y, 0);
            Vector3 right = angle * new Vector3(info.tileSize.x, 0, 0);
            Vector3 back = angle * new Vector3(0, 0, info.tileSize.z);

            // Convert this back to being on the outside, as having the actual hard position will allow me to do some sort of hash to determine a position.
            GameObject innerWall = new GameObject("PerturbedWall");
            innerWall.transform.SetParent(wall.transform, false);
            VerticesPerSide = Mathf.Max(VerticesPerSide, 2);
            DrawPerturbedWall(info, innerWall, up, right, back);
        }

        void DrawPerturbedWall(WallInfo info, GameObject wall, Vector3 up, Vector3 right, Vector3 back) {
            var mesher = new Mesher(wall);
            Vector3 basePos = info.position + -right / 2f + back / 2f;
            Vector3 xDelta = right / (VerticesPerSide - 1);
            Vector3 yDelta = up / (VerticesPerSide - 1);

            float leftMod = GetLeftMod(info.wallDraws);
            float rightMod = GetRightMod(info.wallDraws);

            // Vertices & Mesh
            int idx = 0;
            for (float x = 0; x < VerticesPerSide; x++) {
                float xOffset = x == 0 ? leftMod : (x == VerticesPerSide - 1 ? rightMod : 0);
                for (float y = 0; y < VerticesPerSide; y++) {
                    bool perturb = !(ConvergeAtEdges && (x == 0 || y == 0 || x == VerticesPerSide - 1 || y == VerticesPerSide - 1));
                    Vector3 vPos = basePos + x * xDelta + y * yDelta;
                    float inset = perturb ? InsetForPoint(vPos) : 0;
                    vPos = !perturb ? vPos : vPos - back * inset + inset * xOffset * right;
                    mesher.AddVert(TextureView, vPos, new Vector2(x / (VerticesPerSide - 1), y / (VerticesPerSide - 1)));
                    ++idx;
                }
            }

            // Triangles
            for (int x = 0; x < VerticesPerSide - 1; x++) {
                for (int y = 0; y < VerticesPerSide - 1; y++) {
                    int baseNum = y * VerticesPerSide + x;
                    mesher.AddTriangle(TextureView, baseNum, baseNum + 1, baseNum + 1 + VerticesPerSide);
                    mesher.AddTriangle(TextureView, baseNum, baseNum + 1 + VerticesPerSide, baseNum + VerticesPerSide);
                }
            }
            mesher.Finish();
        }

        float InsetForPoint(Vector3 position) {
            int modVert = VerticesPerSide - 1;
            float modifiedPos = (position.x * modVert * 13 + position.y * modVert * 29 + position.z * modVert * 113);
            return modifiedPos / 811f % 1 * MaxInset;
        }
    }
}