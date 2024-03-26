using DungeonestCrab.Dungeon;
using Pomerandomian;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
	[InlineEditor]
	public abstract class IWallDrawer : ScriptableObject {
		public abstract void DrawWall(Transform parent, IRandom random, TileSpec tile, Vector3 position, Vector3Int tileSize, float rot, float minY, float maxY);
    }
}