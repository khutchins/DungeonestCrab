using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KH.AStar;
using KH.Noise;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator {
	/// <summary>
	/// Will connect all regions in a map. Does not require the
	/// dungeon to be filled.
	/// </summary>
	public class AStarCombiner : AStarAbstractCombiner {

		public AStarCombiner(TerrainSO terrain, float chanceOfExtraPath = 0F, float floorCost = 1, float additionalCarveCost = 0, INoiseSource carveNoise = null, float carveNoiseMultiplier = 1) 
			: base(terrain, chanceOfExtraPath, floorCost, additionalCarveCost, carveNoise, carveNoiseMultiplier) {
		}

		public override float CostForTile(int x, int y, TheDungeon gen) {
			float cost;
			TileSpec spec = gen.GetTileSpec(x, y);
			// Already carved, should be cheap
			if (spec.Walkable) cost = floorCost;
			// Can't be walked or changed, cannot traverse
			else if (spec.Immutable) cost = -1;
			// Figure out cost to carve
			else {
				cost = gen.TileCarvingCost(spec);
				if (cost < 0) {
					return cost;
				}
				if (gen.GetTile(x, y) != Tile.Floor) cost += additionalCarveCost;
				if (carveNoise != null) cost += carveNoise.At(x, y) * carveNoiseMultiplier;
			}
			return cost;
		}
	}
}