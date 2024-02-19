using DungeonestCrab.Dungeon;
using Pomerandomian;
using System;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
	[CreateAssetMenu(menuName = "Dungeon/WallDrawer/Tile")]
	public class WallDrawerTiles : IWallDrawer {

		public GameObject Wall;
		public GameObject LowerWall;

		public override void DrawWall(Transform parent, IRandom random, TileSpec tile, Vector3 position, Vector3Int tileSize, float rot, float minY, float maxY) {
			if (LowerWall == null) LowerWall = Wall;
			int tileHeight = tileSize.y;
			for (float i = Math.Min(0, maxY); i > minY; i -= tileHeight) {
				float y = position.y + i - tileHeight;
				GameObject go = Instantiate(LowerWall, new Vector3(position.x, y, position.z), Quaternion.Euler(0, rot, 0), parent);
				go.name = $"Wall ({tile.Coords.x}, {tile.Coords.y} Z: {y})";
			}
			for (float i = Math.Max(minY, 0); i < maxY; i += tileHeight) {
				float y = position.y + i;
				GameObject go = Instantiate(Wall, new Vector3(position.x, y, position.z), Quaternion.Euler(0, rot, 0), parent);
				go.name = $"Wall ({tile.Coords.x}, {tile.Coords.y} Z: {y})";
			}
		}
	}
}