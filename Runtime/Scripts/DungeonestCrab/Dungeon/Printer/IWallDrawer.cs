using DungeonestCrab.Dungeon;
using Pomerandomian;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
	public abstract class IWallDrawer : ScriptableObject {
		public abstract void DrawWall(Transform parent, IRandom random, TileSpec tile, Vector3 position, float rot, float minY, float maxY);
    }
}