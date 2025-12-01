namespace DungeonestCrab.Dungeon.Generator {
    public static class RoomSpecHelpers {
		public static void Outline(Stamp stamp) {
			int h = stamp.H;
			int w = stamp.W;
			for (int y = 0; y < h; y++) {
				for (int x = 0; x < w; x++) {
					if (stamp.At(x, y) != Tile.Unset) continue;
					if (stamp.At(x - 1, y, Tile.Unset) == Tile.Floor
						|| stamp.At(x, y - 1, Tile.Unset) == Tile.Floor
						|| stamp.At(x, y + 1, Tile.Unset) == Tile.Floor
						|| stamp.At(x + 1, y, Tile.Unset) == Tile.Floor) {
						stamp.MaybeSetAt(x, y, Tile.Wall);
					}
				}
			}
		}

		public static Tile TileAt(Tile[,] tiles, int x, int y, Tile defaultValue) {
			if (x < 0 || y < 0 || x >= tiles.GetLength(1) || y >= tiles.GetLength(0)) return defaultValue;
			return tiles[y, x];
		}
	}
}