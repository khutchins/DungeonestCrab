using DungeonestCrab.Dungeon;
using Pomerandomian;
using System;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
	[CreateAssetMenu(menuName = "Dungeon/WallDrawer/Tile")]
	public class WallDrawerTiles : IWallDrawer {

		public GameObject LowerWall;
		public GameObject Wall;

		public override void DrawWall(Transform parent, IRandom random, TileSpec tile, Vector3 position, float rot, float minY, float maxY) {
			for (float i = Math.Min(0, maxY); i > minY; i--) {
				float y = position.y + i - 1;
				GameObject go = Instantiate(LowerWall, new Vector3(position.x, y, position.z), Quaternion.Euler(0, rot, 0), parent);
				go.name = $"Wall ({position.x}, {position.z} Z: {y})";
			}
			for (float i = Math.Max(minY, 0); i < maxY; i++) {
				float y = position.y + i;
				GameObject go = Instantiate(Wall, new Vector3(position.x, y, position.z), Quaternion.Euler(0, rot, 0), parent);
				go.name = $"Wall ({position.x}, {position.z} Z: {y})";
			}
		}
	}
}