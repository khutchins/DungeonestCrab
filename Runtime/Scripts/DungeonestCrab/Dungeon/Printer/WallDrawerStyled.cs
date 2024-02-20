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

		public override void DrawWall(Transform parent, IRandom random, TileSpec tile, Vector3 position, Vector3Int tileSize, float rot, float minY, float maxY) {
			string style = tile.GetTagType(TileSpec.STYLE_PREFIX);

			Override over = Styles.FirstOrDefault(x => x.Style == style);
			if (over != null) {
				over.Drawer.DrawWall(parent, random, tile, position, tileSize, rot, minY, maxY);
			} else {
				Default.DrawWall(parent, random, tile, position, tileSize, rot, minY, maxY);
			}
		}
	}
}