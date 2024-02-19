using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
	[CreateAssetMenu(menuName = "Dungeon/Spec/Source - Maze")]
	public class SourceMazeSO : SourceSO {
		/// <param name="straightBias"></param>
		/// <param name="conservative"></param>
		/// <param name="braidPercent"></param>
		[Range(0f, 1f)]
		[Tooltip("Bias towards straight lines in the maze.")]
		public float StraightBias = 0.5f;
		[Range(0f, 1f)]
		[Tooltip("Percent of dead ends that should be turned into loops.")]
		public float BraidPercent = 0f;
		[Tooltip("Whether the maze should attempt to avoid accidentally exposing its innards to an existing floor area.")]
		public bool Conservative = false;

		public override ISource ToSource() {
			return new SourceMaze(TileToSet, StraightBias, Conservative, BraidPercent);
		}
	}
}