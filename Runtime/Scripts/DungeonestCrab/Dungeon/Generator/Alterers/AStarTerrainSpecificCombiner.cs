using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KH.Noise;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator {
	public class AStarTerrainSpecificCombiner : AStarAbstractCombiner {
		private readonly TerrainSO _terrainToCarveThrough;

		public AStarTerrainSpecificCombiner(TerrainSO terrain, TerrainSO terrainToCarveThrough, float chanceOfExtraPath = 0F, float floorCost = 1, float additionalCarveCost = 0, INoiseSource carveNoise = null, float carveNoiseMultiplier = 1) 
			: base(terrain, chanceOfExtraPath, floorCost, additionalCarveCost, carveNoise, carveNoiseMultiplier) {
			_terrainToCarveThrough = terrainToCarveThrough;
		}

		public override float CostForTile(int x, int y, TheDungeon gen) {
			TileSpec tile = gen.GetTileSpec(x, y);
			TerrainSO terrain = tile.Terrain;
			float cost;
			// Already carved, should be cheap
			if (tile.Walkable) cost = floorCost;
			// Can't be walked or changed, cannot traverse
			else if (tile.Immutable) cost = -1;
			else if (terrain != _terrainToCarveThrough) cost = -1;
			// Figure out cost to carve
			else {
				cost = gen.TileCarvingCost(tile);
				// A cost of less than zero should not be carved, so return immediately.
				if (cost < 0) {
					return cost;
				}
				if (gen.GetTile(x, y) != Tile.Floor) cost += additionalCarveCost;
				if (carveNoise != null) cost += carveNoise.At(x, y);
			}
			return cost;
		}
	}
}