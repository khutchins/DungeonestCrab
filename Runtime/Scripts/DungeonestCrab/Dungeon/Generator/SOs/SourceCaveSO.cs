using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "Dungeon/Spec/Source - Cave")]
	public class SourceCaveSO : SourceSO {
		[Range(0f, 1f)]
		public float InitialTileOdds = 0.5f;
		[Range(1, 8)]
		public int MinNeighbors = 3;
		[Range(1, 8)]
		public int MaxNeighbors = 8;
		[Range(0, 10)]
		public int Iterations = 5;
		public bool Invert = false;

		public override ISource ToSource() {
			return new SourceCave(TileToSet, InitialTileOdds, MinNeighbors, MaxNeighbors, Invert, Iterations);
        }
    }
}