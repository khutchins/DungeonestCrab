using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
	[CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Wall - Styles")]
	public class WallDrawerStyled : IWallDrawer {
		[SerializeField] IWallDrawer Default;
		[SerializeField] Override[] Styles;

		[System.Serializable]
		public class Override {
			public string Style;
			public IWallDrawer Drawer;
		}

		public override void DrawWall(WallInfo info) {
			string style = info.tileSpec.GetTagType(TileSpec.STYLE_PREFIX);

			Override over = Styles.FirstOrDefault(x => x.Style == style);
			if (over != null) {
				over.Drawer.DrawWall(info);
			} else {
				Default.DrawWall(info);
			}
		}
	}
}