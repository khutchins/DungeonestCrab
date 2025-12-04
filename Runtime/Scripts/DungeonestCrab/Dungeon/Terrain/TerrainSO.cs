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

		[Header("Attributes")]
		[Tooltip("Whether or not the terrain has a ceiling. This can be overridden to false by various traits.")]
		public bool HasCeiling = true;
        [Tooltip("How far upward the ceiling tiles should be placed. A value of 1 puts the ceiling 1 unit above the player. Should only be positive.")]
        public int CeilingOffset = 1;
        [Tooltip("How far downward the draw as floor wall tiles should be placed. A value of 0 puts the ground at the player's feet. Should only be positive.")]
		public float GroundOffset = 0;
		[Tooltip("How far up the wall tiles should extend.")]
		public int WallHeight = 1;
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

        [Title("Features")]
        [SerializeReference]
        [ListDrawerSettings(ShowIndexLabels = false, ShowFoldout = true)]
        public List<TerrainMixin> Mixins = new List<TerrainMixin>();

        public T GetMixin<T>() where T : TerrainMixin {
            for (int i = 0; i < Mixins.Count; i++) {
                if (Mixins[i] is T t) return t;
            }
            return null;
        }

        public float TileCarvingCost(Tile type) {
            var pathing = GetMixin<TerrainCarvingMixin>();

            // If no mixin, return default hardcoded values
            if (pathing == null) return type == Tile.Wall ? 6 : 1;

            return type switch {
                Tile.Floor => pathing.FloorCarveCost,
                Tile.Wall => pathing.WallCarveCost,
                _ => 6,
            };
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