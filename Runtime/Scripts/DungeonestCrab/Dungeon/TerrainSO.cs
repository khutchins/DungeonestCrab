using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pomerandomian;
using KH.Audio;
using Sirenix.OdinInspector;
using DungeonestCrab.Dungeon.Printer;

namespace DungeonestCrab.Dungeon {
	[CreateAssetMenu(menuName = "DungeonestCrab/Terrain")]
	public class TerrainSO : ScriptableObject {

		public enum FloorStyle {
			Single,
			Directional
		}

		public string ID;
		public virtual AudioEvent GetFootstepSound() { return null; }

		public virtual Sprite GetWallSprite(int style) { return null; }
		public virtual Sprite GetFloorSprite(int style) { return null; }

		[Header("Attributes")]
		[Tooltip("Whether or not the terrain has a ceiling. This can be overridden to false by various traits.")]
		public bool HasCeiling = true;
		[Tooltip("How far downward the draw as floor wall tiles should be placed. A value of 0 puts the ground at the player's feet. Should only be positive.")]
		public float GroundOffset = 0;
		[Tooltip("How far up the wall tiles should extend.")]
		public int WallHeight = 2;
		[ShowIf("HasCeiling")]
		[Tooltip("How far upward the ceiling tiles should be placed. A value of 1 puts the ceiling 1 unit above the player. Should only be positive.")]
		public int CeilingOffset = 1;
		[Tooltip("Whether or not it should use its floor texture for the lower wall it abutts.")]
		public bool HasInvasiveLowerWall = false;
		[Tooltip("Whether or not offset tiles should have shadows drawn on them to give the impression of an endless stretch.")]
		public bool DrawOffsetShadow = false;
		[Tooltip("Whether or not the wall of this terrain should be rendered like a floor.")]
		public bool DrawWallAsFloor = false;

		[Header("Drawing")]
		public IWallDrawer WallDrawer;
		[Tooltip("Top section of the wall, only necessary if the wall is shorter than the ceiling and the player can peek over it.")]
		public IFlatDrawer WallCapDrawer;
		[ShowIf("DrawWallAsFloor")]
		public IFlatDrawer WallFloorDrawer;
		public IFlatDrawer FloorDrawer;
		[ShowIf("HasCeiling")]
		public IFlatDrawer CeilingDrawer;

		[Header("Carve Costs")]
		[Tooltip("Whether or not the terrain specifies custom carving costs.")]
		public bool OverrideTileCosts = false;
		[ShowIf("OverrideTileCosts")]
		[Tooltip("How much it costs to carve a path through wall tiles of this terrain.")]
		public float WallCarveCost = 6;
		[ShowIf("OverrideTileCosts")]
		[Tooltip("How much it costs to carve a path through floor tiles of this terrain.")]
		public float FloorCarveCost = 1;

		public float TileCarvingCost(Tile type, bool immutable) {
			if (immutable) {
				return -1;
			}
			switch (type) {
				case Tile.Floor:
					return OverrideTileCosts ? FloorCarveCost : 1;
				case Tile.Wall:
					return OverrideTileCosts ? WallCarveCost : 6;
				default:
					return 6;
			}
		}

		public static char GenericCharForTile(Tile type) {
			switch (type) {
				case Tile.Floor:
					return '.';
				case Tile.Wall:
					return 'X';
				default:
					return '?';
			}
		}

		public bool ShouldDrawAsFloor(Tile type) {
			if (type == Tile.Wall) {
				return DrawWallAsFloor;
			}
			return true;
		}
	}
}