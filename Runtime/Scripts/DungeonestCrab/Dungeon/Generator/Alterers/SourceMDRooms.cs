using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator {
    [System.Serializable]
    public class SourceMDRooms : ISource {
        [Header("Grid Config")]
        public readonly int GridColumns = 4;
        public readonly int GridRows = 3;

        [Header("Room Logic")]
        [Tooltip("Percentage of cells to target for rooms.")]
        [Range(0, 1)] public readonly float RoomDensity = 0.5f;

        [Tooltip("Represents walker steps as a % of total grid cells.")]
        [Range(0f, 2f)]
        public readonly float Connectivity = 0.6f;

        public readonly bool AllowDeadEnds = true;

        [Range(0, 1)] public readonly float ImperfectionChance = 0.4f;
        [Range(0, 1)] public readonly float MergeChance = 0.05f;

        private class Cell {
            public int X, Y;
            public RectInt Bounds;

            public RectInt RoomRect;
            public Vector2Int CrossroadPoint;
            public bool IsRoom;
            public bool IsMerged;
            public RectInt MergedRect;

            public bool IsConnectedToAnything = false;
            public HashSet<int> Connections = new HashSet<int>();
        }

        public SourceMDRooms(Tile tile, int cols, int rows, float density = 0.7f, float connectivity = 0.6f, 
                bool allowDeadEnds = true, float imperfectionChance = 0.4f, float mergeChance = 0.05f) : base(tile) {
            GridColumns = cols;
            GridRows = rows;
            RoomDensity = density;
            Connectivity = connectivity;
            AllowDeadEnds = allowDeadEnds;
            ImperfectionChance = imperfectionChance;
            MergeChance = mergeChance;
        }

        public override void Generate(Stamp stamp, IRandom rand) {
            // Initialize the room grid.
            int cellW = stamp.W / GridColumns;
            int cellH = stamp.H / GridRows;
            if (cellW < 5 || cellH < 5) {
                Debug.LogWarning("Map too small for grid size.");
                return;
            }

            Cell[,] grid = new Cell[GridColumns, GridRows];
            List<Cell> cells = new List<Cell>();

            for (int x = 0; x < GridColumns; x++) {
                for (int y = 0; y < GridRows; y++) {
                    grid[x, y] = new Cell {
                        X = x,
                        Y = y,
                        Bounds = new RectInt(x * cellW, y * cellH, cellW, cellH)
                    };
                    cells.Add(grid[x, y]);
                }
            }

            // Get how many rooms we're targeting.
            int totalCells = GridColumns * GridRows;
            int targetRooms = Mathf.FloorToInt(totalCells * RoomDensity) + rand.Next(0, 3);
            targetRooms = Mathf.Clamp(targetRooms, 0, totalCells);

            // Assign initial rooms
            var shuffled = cells.Shuffle(rand).ToList();
            for (int i = 0; i < targetRooms; i++) {
                shuffled[i].IsRoom = true;
            }

            // Ensure there are at least two rooms.
            int roomCount = cells.Count(c => c.IsRoom);
            if (roomCount < 2) {
                for (int i = 0; i < 200; i++) {
                    if (roomCount >= 2) break;
                    foreach (var c in cells) {
                        if (roomCount >= 2) break;
                        if (rand.WithPercentChance(0.6f)) { // 60% chance to flip
                            if (!c.IsRoom) {
                                c.IsRoom = true;
                                roomCount++;
                            }
                        }
                    }
                }
            }

            foreach (var cell in cells) {
                int minX = cell.Bounds.x + 2;
                int minY = cell.Bounds.y + 2;
                int limitW = cell.Bounds.width - 4;
                int limitH = cell.Bounds.height - 3;

                if (cell.IsRoom) {
                    // I could make this configurable, but I have too many knobs as it is.
                    int w = rand.Next(System.Math.Min(5, limitW - 1), limitW);
                    int h = rand.Next(System.Math.Min(4, limitH - 1), limitH);

                    if (w % 2 == 0 && w < limitW - 1) w++;
                    if (h % 2 == 0 && h < limitH - 1) h++;

                    if (w > h * 1.5f) w = Mathf.FloorToInt(h * 1.5f);
                    if (h > w * 1.5f) h = Mathf.FloorToInt(w * 1.5f);

                    int posX = minX + rand.Next(0, limitW - w);
                    int posY = minY + rand.Next(0, limitH - h);

                    cell.RoomRect = new RectInt(posX, posY, w, h);
                } else {
                    // Crossroads are nodes placed in a cell for the connector to use as an anchor.
                    int leftMargin = (cell.X == 0) ? 1 : 2;
                    int topMargin = (cell.Y == 0) ? 1 : 2;
                    int rightMargin = (cell.X == GridColumns - 1) ? 2 : 4;
                    int bottomMargin = (cell.Y == GridRows - 1) ? 2 : 4;

                    int maxHOffset = limitW;
                    int maxVOffset = limitH;

                    int xRange = (minX + maxHOffset - rightMargin - 1) - (minX + leftMargin);
                    int yRange = (minY + maxVOffset - bottomMargin - 1) - (minY + topMargin);

                    int posX = minX + leftMargin + (xRange > 0 ? rand.Next(0, xRange) : 0);
                    int posY = minY + topMargin + (yRange > 0 ? rand.Next(0, yRange) : 0);

                    cell.CrossroadPoint = new Vector2Int(posX, posY);
                }
            }

            int steps = Mathf.CeilToInt(totalCells * Connectivity);
            steps = Mathf.Max(2, steps);

            // Randomly walk, connecting to neighbor.
            Cell current = rand.From(cells);
            for (int i = 0; i < steps; i++) {
                int startDir = rand.Next(0, 4);
                for (int d = 0; d < 4; d++) {
                    int dir = (startDir + d) % 4;
                    Cell neighbor = GetNeighbor(grid, current, dir);
                    if (neighbor != null) {
                        Connect(current, neighbor, dir);
                        current = neighbor;
                        break;
                    }
                }
            }

            // This forces dead end crossroads to connect to at least one other neighbor.
            if (!AllowDeadEnds) {
                // I can remove this - it lets me more easily visualize the
                // impact of this option by eliminating it as something that
                // consumes the default random.
                IRandom childRandom = rand.Split(0);
                foreach (var c in cells) {
                    if (c.IsRoom) continue;
                    if (c.IsConnectedToAnything && c.Connections.Count == 1) {
                        // Connect to a random neighbor
                        List<int> potentials = new List<int> { 0, 1, 2, 3 };
                        potentials.Remove(c.Connections.First());
                        int dir = childRandom.From(potentials);
                        Cell n = GetNeighbor(grid, c, dir);
                        if (n != null) Connect(c, n, dir);
                    }
                }
            }

            // Draw hallways
            foreach (var cell in cells) {
                if (cell.Connections.Contains(1)) DrawHallway(stamp, cell, grid[cell.X + 1, cell.Y], true, rand);
                if (cell.Connections.Contains(2)) DrawHallway(stamp, cell, grid[cell.X, cell.Y + 1], false, rand);
            }

            // Room merging
            foreach (var cell in cells) {
                if (cell.IsRoom && cell.IsConnectedToAnything && !cell.IsMerged && rand.WithPercentChance(MergeChance)) {
                    int dir = rand.Next(0, 4);
                    Cell neighbor = GetNeighbor(grid, cell, dir);

                    if (neighbor != null && neighbor.IsRoom && neighbor.IsConnectedToAnything && !neighbor.IsMerged) {
                        RectInt r1 = cell.RoomRect;
                        RectInt r2 = neighbor.RoomRect;

                        int x0 = Mathf.Min(r1.xMin, r2.xMin);
                        int x1 = Mathf.Max(r1.xMax, r2.xMax);
                        int y0 = Mathf.Min(r1.yMin, r2.yMin);
                        int y1 = Mathf.Max(r1.yMax, r2.yMax);

                        RectInt merged = new RectInt(x0, y0, x1 - x0, y1 - y0);

                        cell.IsMerged = true;
                        cell.MergedRect = merged;
                        neighbor.IsMerged = true;
                        neighbor.MergedRect = merged;
                    }
                }
            }

            // Ensure all cells are connected (unconnected crossroads are dropped).
            foreach (var cell in cells) {
                if (!cell.IsConnectedToAnything && !cell.IsMerged) {
                    if (cell.IsRoom) {
                        // Pick random dir, check if neighbor is connected
                        int startDir = rand.Next(0, 4);
                        for (int d = 0; d < 4; d++) {
                            int dir = (startDir + d) % 4;
                            Cell n = GetNeighbor(grid, cell, dir);
                            if (n != null && n.IsConnectedToAnything) {
                                Connect(cell, n, dir);
                                bool horizontal = (dir == 1 || dir == 3);
                                if (dir == 1 || dir == 2) DrawHallway(stamp, cell, n, horizontal, rand);
                                else DrawHallway(stamp, n, cell, horizontal, rand);
                                break;
                            }
                        }
                    }
                }
            }

            // Actually stamp the rooms
            foreach (var cell in cells) {
                // Isolated rooms are not propagated
                if (!cell.IsConnectedToAnything && !cell.IsMerged) continue;

                if (cell.IsRoom) {
                    RectInt rectToDraw = cell.IsMerged ? cell.MergedRect : cell.RoomRect;
                    CarveRect(stamp, rectToDraw);

                    if (!cell.IsMerged && rand.WithPercentChance(ImperfectionChance)) {
                        ApplyImperfections(stamp, rectToDraw, rand);
                    }
                } else {
                    stamp.MaybeSetAt(cell.CrossroadPoint, _tileToSet);
                }
            }
        }
        private void Connect(Cell a, Cell b, int dirFromA) {
            a.IsConnectedToAnything = true;
            b.IsConnectedToAnything = true;
            a.Connections.Add(dirFromA);
            b.Connections.Add((dirFromA + 2) % 4);
        }

        private Cell GetNeighbor(Cell[,] grid, Cell c, int dir) {
            int x = c.X;
            int y = c.Y;
            if (dir == 0) y--;      // N
            if (dir == 1) x++;      // E
            if (dir == 2) y++;      // S
            if (dir == 3) x--;      // W

            if (x >= 0 && x < GridColumns && y >= 0 && y < GridRows) {
                return grid[x, y];
            }
            return null;
        }

        private void DrawHallway(Stamp stamp, Cell start, Cell end, bool horizontal, IRandom rand) {
            Vector2Int pStart = GetConnectionPoint(start, rand);
            Vector2Int pEnd = GetConnectionPoint(end, rand);

            List<Vector2Int> path = new List<Vector2Int>();
            Vector2Int cursor = pStart;

            if (horizontal) {
                int boundaryX = start.Bounds.xMax;
                while (cursor.x < boundaryX) {
                    stamp.MaybeSetAt(cursor, _tileToSet);
                    cursor.x++;
                }
                while (cursor.y != pEnd.y) {
                    stamp.MaybeSetAt(cursor, _tileToSet);
                    cursor.y += System.Math.Sign(pEnd.y - cursor.y);
                }
                while (cursor.x < pEnd.x) {
                    stamp.MaybeSetAt(cursor, _tileToSet);
                    cursor.x++;
                }
            } else { // Vertical
                int boundaryY = start.Bounds.yMax;
                while (cursor.y < boundaryY) {
                    stamp.MaybeSetAt(cursor, _tileToSet);
                    cursor.y++;
                }
                while (cursor.x != pEnd.x) {
                    stamp.MaybeSetAt(cursor, _tileToSet);
                    cursor.x += System.Math.Sign(pEnd.x - cursor.x);
                }
                while (cursor.y < pEnd.y) {
                    stamp.MaybeSetAt(cursor, _tileToSet);
                    cursor.y++;
                }
            }
            stamp.MaybeSetAt(cursor, _tileToSet);
        }

        private Vector2Int GetConnectionPoint(Cell c, IRandom rand) {
            if (!c.IsRoom) return c.CrossroadPoint;

            RectInt r = c.IsMerged ? c.MergedRect : c.RoomRect;

            // Safety check for tiny rooms
            int minX = r.xMin + 1;
            int maxX = r.xMax - 1;
            int minY = r.yMin + 1;
            int maxY = r.yMax - 1;

            if (minX >= maxX) minX = maxX - 1;
            if (minY >= maxY) minY = maxY - 1;

            return new Vector2Int(
                rand.Next(minX, maxX),
                rand.Next(minY, maxY)
            );
        }

        private void CarveRect(Stamp stamp, RectInt r) {
            for (int x = r.x; x < r.xMax; x++) {
                for (int y = r.y; y < r.yMax; y++) {
                    stamp.MaybeSetAt(x, y, _tileToSet);
                }
            }
        }

        private void ApplyImperfections(Stamp stamp, RectInt r, IRandom rand) {
            // Make some of the rooms messier. I should probably have this take
            // room suppliers instead of just doing this step, but that's
            // future me's problem.

            // For right now, just randomly clear out some corners.
            if (rand.NextBool()) stamp.MaybeSetAt(r.x, r.y, Tile.Unset);
            if (rand.NextBool()) stamp.MaybeSetAt(r.xMax - 1, r.y, Tile.Unset);
            if (rand.NextBool()) stamp.MaybeSetAt(r.x, r.yMax - 1, Tile.Unset);
            if (rand.NextBool()) stamp.MaybeSetAt(r.xMax - 1, r.yMax - 1, Tile.Unset);
        }
    }
}