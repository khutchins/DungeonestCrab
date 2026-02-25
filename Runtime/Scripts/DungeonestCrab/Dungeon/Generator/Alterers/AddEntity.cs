using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
	public class AddEntity : IEntityAdder {
		private readonly EntitySource _source;
		private readonly bool _placeToNotBlockDungeon;
		private readonly bool _avoidAdjacency;
		private readonly int _minRequired;
		private readonly int _targetAmt;
		private readonly float _chanceToPlace;
		private readonly IMatcher _matcher;

		public AddEntity(EntitySource source, IMatcher matcher, bool placeToNotBlockDungeon, bool avoidAdjacency, int minRequired, int targetAmt, float chanceToPlace = 1F) {
			_source = source;
			this._placeToNotBlockDungeon = placeToNotBlockDungeon;
			this._avoidAdjacency = avoidAdjacency;
			this._minRequired = minRequired;
			this._targetAmt = targetAmt;
			this._chanceToPlace = chanceToPlace;
			this._matcher = matcher;
		}

		public override bool Modify(TheDungeon generator, IRandom rand) {
			int actualTarget = (int)(_targetAmt * _chanceToPlace);
			return PlaceMany(generator, rand, _source, _matcher, _placeToNotBlockDungeon, _minRequired, actualTarget, _avoidAdjacency);
		}
	}
}