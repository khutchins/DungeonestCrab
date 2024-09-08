using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Wall - Specified")]
    public class WallDrawerSpecified : WallDrawerSpecifiedBase {
        [SerializeField] WallSpec[] WallConfiguration;

        protected override WallSpec[] GetWallConfiguration() {
            return WallConfiguration;
        }
    }
}