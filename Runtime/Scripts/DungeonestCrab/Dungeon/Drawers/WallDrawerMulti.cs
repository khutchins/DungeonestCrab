using DungeonestCrab.Dungeon.Printer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Wall - Multi")]
    public class WallDrawerMulti : IWallDrawer {
        [InlineEditor][SerializeField] IWallDrawer[] WallDrawers;

        public override void DrawWall(WallInfo wallInfo) {
            foreach (var wall in WallDrawers) {
                if (wall != null) {
                    wall.DrawWall(wallInfo);
                }
            }
        }
    }
}