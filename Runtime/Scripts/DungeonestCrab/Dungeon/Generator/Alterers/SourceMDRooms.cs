using System.Collections.Generic;
using UnityEngine;
using Pomerandomian;
using DungeonestCrab.Dungeon;
using System.Linq;

namespace DungeonestCrab.Dungeon.Generator {
	[System.Serializable]
	public class SourceMDRooms : ISource {
		[SerializeField] Vector2Int _quadrantSize = new Vector2Int(7, 7);
		[SerializeField] Vector2Int _minSize = new Vector2Int(3, 3);
		[SerializeField] Vector2Int _maxSize = new Vector2Int(5, 5);
		[SerializeField] float _percentRooms = 0.7f;

        public SourceMDRooms(Vector2Int quadrantSize, Vector2Int minSize, Vector2Int maxSize, float percentRooms) : base(Tile.Floor) {
            _quadrantSize = quadrantSize;
            _minSize = minSize;
            _maxSize = maxSize;
            _percentRooms = percentRooms;
        }

        public Stamp CreateRoom(RoomStrategy spec, int x, int y, int w, int h, IRandom rand) {
			Stamp stamp = new Stamp(x, y, w, h);
			spec.StampRoom(stamp, rand);
			return stamp;
		}

		public override void Generate(Stamp stamp, IRandom rand) {
			_maxSize.x = Mathf.Min(_quadrantSize.x - 2, _maxSize.x);
			_maxSize.y = Mathf.Min(_quadrantSize.y - 2, _maxSize.y);
			_minSize.x = Mathf.Clamp(_minSize.x, 1, _maxSize.x);
			_minSize.y = Mathf.Clamp(_minSize.y, 1, _maxSize.y);

			Vector2Int cells = new Vector2Int(stamp.W / _quadrantSize.x, stamp.H / _quadrantSize.y);
			int totalCells = cells.x * cells.y;
			int roomCount = Mathf.RoundToInt(totalCells * _percentRooms);
			Vector2IntCoordExtensions.Range(0, 0, cells.x, cells.y);
			// This produces all cell coordinates in random order.
			// Any cell at idx < roomCount is a room. Everything else is an anchor.
			List<Vector2Int> allCells = cells.Range().Shuffle(rand).ToList();

			// TODO: Do it
		}
	}
}