using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
	public class AddEntityAtPoints : IEntityAdder {

		public delegate List<Vector2Int> PointCollector(TheDungeon generator, IRandom random);

		private readonly EntitySource _source;
		private readonly Entity.CodeOnInstantiate _code;
		private readonly PointCollector _collector;

		public AddEntityAtPoints(EntitySource source, PointCollector collector) {
			_source = source;
			_collector = collector;
		}

		public override bool Modify(TheDungeon generator, IRandom rand) {
			PlaceAt(generator, rand, _source, _collector.Invoke(generator, rand));
			return true;
		}
	}
}