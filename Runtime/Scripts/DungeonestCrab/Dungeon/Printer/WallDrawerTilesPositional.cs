using DungeonestCrab.Dungeon;
using Pomerandomian;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
	[CreateAssetMenu(menuName = "Dungeon/WallDrawer/TilePositional")]
	public class WallDrawerTilesPositional : IWallDrawer {

		public static readonly int SPECIAL_NORTH = 1 << 1;
		public static readonly int SPECIAL_SOUTH = 1 << 2;
		public static readonly int SPECIAL_EAST = 1 << 3;
		public static readonly int SPECIAL_WEST = 1 << 4;

		public GameObject LowestWall;
		public GameObject LowestWallSpecial;
		public GameObject Wall;

		public override void DrawWall(Transform parent, IRandom random, TileSpec tile, Vector3 position, float rot, float minY, float maxY) {
			if (rot == 0 && FlagSet(tile.Style, SPECIAL_NORTH)
				|| (rot == 90 && FlagSet(tile.Style, SPECIAL_EAST))
				|| (rot == 270 && FlagSet(tile.Style, SPECIAL_WEST))
				|| (rot == 180 && FlagSet(tile.Style, SPECIAL_SOUTH))) {
				GameObject go = Instantiate(LowestWallSpecial, new Vector3(position.x, position.y + minY, position.z), Quaternion.Euler(0, rot, 0), parent);
				go.name = $"Wall ({position.x}, {position.z} Z: {position.y + minY})";
			} else {
				GameObject go = Instantiate(LowestWall, new Vector3(position.x, position.y + minY, position.z), Quaternion.Euler(0, rot, 0), parent);
				go.name = $"Wall ({position.x}, {position.z} Z: {position.y + minY})";
			}
			for (float i = minY + 1; i < maxY; i++) {
				GameObject go = Instantiate(Wall, new Vector3(position.x, position.y + i, position.z), Quaternion.Euler(0, rot, 0), parent);
				go.name = $"Wall ({position.x}, {position.z} Z: {position.y + i})";
			}
		}

		private bool FlagSet(int test, int flag) {
			return (test & flag) != 0;
		}
	}
}