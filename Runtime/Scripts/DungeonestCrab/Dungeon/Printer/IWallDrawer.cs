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
			public int wallDraws;
			public int wallSameTerrainDraws;
			public float minY;
			public float maxY;
        }

		public abstract void DrawWall(WallInfo wallInfo);

		/// <summary>
		/// Defined wall adjacencies. 
		/// TopLeft is the wall on the left side of the adjacent tile's position.
		/// Left is the wall facing in the same direction from the tile on the left.
		/// BottomLeft is the wall between the tile and the tile to its left.
		/// </summary>
		protected enum WallAdjacency {
			TopLeft = 1 << 0,
			Left = 1 << 1,
			BottomLeft = 1 << 2,
			TopRight = 1 << 3,
			Right = 1 << 4,
			BottomRight = 1 << 5
		}

		public static int PackWallAdjacencies(bool topLeft,bool topRight, bool left, bool right, bool bottomLeft, bool bottomRight) {
			int result = 0;
			if (topLeft) result |= (int)WallAdjacency.TopLeft;
			if (topRight) result |= (int)WallAdjacency.TopRight;
			if (left) result |= (int)WallAdjacency.Left;
			if (right) result |= (int)WallAdjacency.Right;
            if (bottomLeft) result |= (int)WallAdjacency.BottomLeft;
            if (bottomRight) result |= (int)WallAdjacency.BottomRight;
            return result;
		}

		protected static bool GetIsAdjacent(int packed, WallAdjacency adjacency) {
			return (packed & (int)adjacency) > 0;
		}
    }
}