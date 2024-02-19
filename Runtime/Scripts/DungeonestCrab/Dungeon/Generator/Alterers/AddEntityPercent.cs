using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;
using DungeonestCrab.Dungeon;

namespace DungeonestCrab.Dungeon.Generator {
	public class AddEntityPercent : IEntityAdder {
		private readonly EntitySource _source;
		private readonly float _chancePerTile;
		private readonly IMatcher _matcher;
		private readonly bool _placeToNotBlockDungeon;

		public AddEntityPercent(EntitySource source, IMatcher tileMatcher, bool placeToNotBlockDungeon, float chancePerTile) {
			_source = source;
			_chancePerTile = chancePerTile;
			_matcher = tileMatcher;
			_placeToNotBlockDungeon = placeToNotBlockDungeon;
		}

		public override bool Modify(TheDungeon generator, IRandom rand) {
			if (_chancePerTile <= 0) return true;
			int qualifiedTiles = 0;
			foreach (TileSpec spec in generator.AllTiles()) {
				if (spec.Immutable) continue;
				if (!_matcher.Matches(spec)) continue;
				qualifiedTiles++;
			}
			int targetAmt = (int)(qualifiedTiles * _chancePerTile);

			return PlaceMany(generator, rand, _source, _matcher, _placeToNotBlockDungeon, 0, targetAmt);
		}
	}
}