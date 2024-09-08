using DungeonestCrab.Dungeon;
using Pomerandomian;
using System;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
	[CreateAssetMenu(menuName = "DungeonestCrab/Drawer/Wall - Tiles")]
	public class WallDrawerTiles : IWallDrawer {

		public GameObject Wall;
		public GameObject LowerWall;

		public override void DrawWall(WallInfo info) {
			if (LowerWall == null) LowerWall = Wall;
			int tileHeight = info.tileSize.y;
			for (float i = Math.Min(0, info.maxY); i > info.minY; i -= tileHeight) {
				float y = info.position.y + i - tileHeight;
				GameObject go = Instantiate(LowerWall, new Vector3(info.position.x, y, info.position.z), Quaternion.Euler(0, info.rotation, 0), info.parent);
				go.name = $"Wall {info.tileSpec.Coords} Z: {y})";
			}
			for (float i = Math.Max(info.minY, 0); i < info.maxY; i += tileHeight) {
				float y = info.position.y + i;
				GameObject go = Instantiate(Wall, new Vector3(info.position.x, y, info.position.z), Quaternion.Euler(0, info.rotation, 0), info.parent);
                go.name = $"Wall {info.tileSpec.Coords} Z: {y})";
            }
		}
	}
}