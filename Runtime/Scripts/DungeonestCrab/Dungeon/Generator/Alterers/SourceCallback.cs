using DungeonestCrab.Dungeon;
using Pomerandomian;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
	/// <summary>
	/// Allows setting a source explicitly through a callback. Useful if a stamp truly is a one-off.
	/// </summary>
	public class SourceCallback : ISource {

		private readonly Action<Stamp, ISeededRandom> _callback;

		/// <summary>
		/// Constructor for the callback source.
		/// </summary>
		/// <param name="callback">Callback executed to perform stamp.</param>
		public SourceCallback(Action<Stamp, ISeededRandom> callback) : base(Tile.Unset) {
			_callback = callback;
		}

		public override void Generate(Stamp stamp, ISeededRandom rand) {
			_callback(stamp, rand);
		}
	}
}