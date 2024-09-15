using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Wall - Pillar")]
    public class WallDrawerPillar : IWallDrawer {
        [Range(0, 0.5f)][SerializeField] float Radius = 0.1f;
        [SerializeField] DrawConditions Conditions = DrawConditions.NormalWall;
        [SerializeField] TextureView TextureView;

        [System.Flags]
        public enum DrawConditions {
            None = 0,
            InnerCorner = 1 << 0,
            OuterCorner = 1 << 1,
            NormalWall = 1 << 2,
        }

        private bool ShouldDraw(WallInfo info) {
            bool bl = GetIsAdjacent(info.wallDraws, WallAdjacency.BottomLeft);
            bool l = GetIsAdjacent(info.wallDraws, WallAdjacency.Left);
            bool tl = GetIsAdjacent(info.wallDraws, WallAdjacency.TopLeft);
            if ((Conditions & DrawConditions.InnerCorner) != 0 && bl) return true;
            if ((Conditions & DrawConditions.NormalWall) != 0 && !bl && l) return true;
            if ((Conditions & DrawConditions.OuterCorner) != 0 && !bl && !l && tl) return true;
            return false;
        }

        public override void DrawWall(WallInfo info) {
            if (!ShouldDraw(info)) return;

            // This is the base.
            GameObject wall = new GameObject($"Wall ({info.tileSpec.Coords.x}, {info.tileSpec.Coords.y})");
            wall.transform.SetParent(info.parent);

            Quaternion angle = Quaternion.AngleAxis(info.rotation, Vector3.up);
            Vector3 up = angle * new Vector3(0, info.tileSize.y, 0);
            Vector3 right = angle * new Vector3(info.tileSize.x, 0, 0);
            Vector3 back = angle * new Vector3(0, 0, info.tileSize.z);

            GameObject innerWall = new GameObject("DrawnWall");
            innerWall.transform.SetParent(wall.transform, false);

            Mesher mesher = new Mesher(wall);
            mesher.GeneratePillar(TextureView.Material, info.position + back * 0.5f - right * 0.5f, Radius * Mathf.Min(info.tileSize.x, info.tileSize.z), info.tileSize.y, TextureView.UV);
            mesher.Finish();
        }
    }
}