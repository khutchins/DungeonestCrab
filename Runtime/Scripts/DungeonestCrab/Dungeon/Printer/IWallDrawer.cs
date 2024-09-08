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
		public struct WallInfo {
			public Transform parent;
            public IRandom random;
			public TileSpec tileSpec;
			public Vector3 position;
			public Vector3Int tileSize;
			public float rotation;
			public float minY;
			public float maxY;
        }

		public abstract void DrawWall(WallInfo wallInfo);
    }
}