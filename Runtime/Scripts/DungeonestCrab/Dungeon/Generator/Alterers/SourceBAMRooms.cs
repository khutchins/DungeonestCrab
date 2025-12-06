using UnityEngine;
using Pomerandomian;
using System.Collections.Generic;

namespace DungeonestCrab.Dungeon.Generator {
    /// <summary>
    /// Rooms that resemble (slightly) the generation of Beneath Apple Manor.
    /// </summary>
    [System.Serializable]
    public class SourceBAMRooms : ISource {
        [Header("Room Density")]
        [Tooltip("Min rooms per tile.")]
        [Range(0f, 0.1f)] public float MinRoomDensity = 0.01f;
        [Tooltip("Max rooms per tile.")]
        [Range(0f, 0.1f)] public float MaxRoomDensity = 0.01f;

        [Header("Room Sizing")]
        [Min(1)] public int MinRoomSize = 3;
        [Min(1)] public int MaxRoomSize = 9;

        [Header("Placement Logic")]
        [Tooltip("How many times to try placing a room to find the 'best' spot (least overlap).")]
        [Min(1)] public int PlacementAttempts = 10;
        [Tooltip("If true, coordinates are forced to odd numbers for cleaner walls.")]
        public bool ForceOddAlignment = true;

        public SourceBAMRooms(Tile tile, float minD, float maxD, int minS, int maxS, int attempts, bool odd) : base(tile) {
            MinRoomDensity = minD;
            MaxRoomDensity = maxD;
            MinRoomSize = minS;
            MaxRoomSize = maxS;
            PlacementAttempts = attempts;
            ForceOddAlignment = odd;
        }

        public override void Generate(Stamp stamp, IRandom rand) {
            int area = stamp.W * stamp.H;
            int minC = Mathf.Max(1, Mathf.FloorToInt(area * MinRoomDensity));
            int maxC = Mathf.Max(minC, Mathf.FloorToInt(area * MaxRoomDensity));

            int count = rand.Next(minC, maxC + 1);

            RectInt firstRoom = GenerateRandomRoom(stamp, rand, true);
            CarveRoom(stamp, firstRoom);

            Vector2Int prevPoint = Vector2Int.RoundToInt(firstRoom.center);
            if (ForceOddAlignment) prevPoint = Align(prevPoint);

            for (int i = 1; i < count; i++) {
                RectInt bestRoom = new RectInt(0, 0, 0, 0);
                int bestScore = int.MaxValue;
                Vector2Int bestCenter = Vector2Int.zero;

                for (int attempt = 0; attempt < PlacementAttempts; attempt++) {
                    RectInt candidate = GenerateRandomRoom(stamp, rand);
                    int score = CalculateOverlapScore(stamp, candidate);

                    if (score < bestScore) {
                        bestScore = score;
                        bestRoom = candidate;
                        bestCenter = Vector2Int.RoundToInt(candidate.center);
                        if (ForceOddAlignment) bestCenter = Align(bestCenter);
                        if (bestScore == 0) break;
                    }
                }

                DigDogleg(stamp, bestCenter, prevPoint);
                CarveRoom(stamp, bestRoom);
                prevPoint = bestCenter;
            }
        }

        private RectInt GenerateRandomRoom(Stamp stamp, IRandom rand, bool forceCenter = false) {
            int w = rand.Next(MinRoomSize, MaxRoomSize + 1);
            int h = rand.Next(MinRoomSize, MaxRoomSize + 1);

            if (ForceOddAlignment) {
                if (w % 2 == 0) w++;
                if (h % 2 == 0) h++;
            }

            int x, y;

            if (forceCenter) {
                Vector2Int center = new Vector2Int(stamp.W / 2, stamp.H / 2);
                if (ForceOddAlignment) center = Align(center);
                x = center.x - w / 2;
                y = center.y - h / 2;
            } else {
                x = rand.Next(1, stamp.W - w - 1);
                y = rand.Next(1, stamp.H - h - 1);
            }

            if (ForceOddAlignment) {
                x = Align(x);
                y = Align(y);
            }

            if (x < 1) x = 1;
            if (y < 1) y = 1;
            if (x + w >= stamp.W) x = stamp.W - w - 1;
            if (y + h >= stamp.H) y = stamp.H - h - 1;

            return new RectInt(x, y, w, h);
        }

        private Vector2Int Align(Vector2Int pt) {
            return new Vector2Int(Align(pt.x), Align(pt.y));
        }

        private int Align(int val) {
            if (val % 2 == 0) return val + 1;
            return val;
        }

        private int CalculateOverlapScore(Stamp stamp, RectInt rect) {
            int score = 0;
            for (int y = rect.y; y < rect.yMax; y++) {
                for (int x = rect.x; x < rect.xMax; x++) {
                    if (stamp.At(x, y) == _tileToSet) {
                        score++;
                    }
                }
            }
            return score;
        }

        private void CarveRoom(Stamp stamp, RectInt rect) {
            for (int y = rect.y; y < rect.yMax; y++) {
                for (int x = rect.x; x < rect.xMax; x++) {
                    stamp.MaybeSetAt(x, y, _tileToSet);
                }
            }
        }

        private void DigDogleg(Stamp stamp, Vector2Int start, Vector2Int end) {
            Vector2Int cursor = start;
            int dirX = (end.x > start.x) ? 1 : -1;
            while (cursor.x != end.x) {
                cursor.x += dirX;
                if (IsFloor(stamp, cursor)) return;
                stamp.MaybeSetAt(cursor, _tileToSet);
            }
            int dirY = (end.y > start.y) ? 1 : -1;
            while (cursor.y != end.y) {
                cursor.y += dirY;
                if (IsFloor(stamp, cursor)) return;
                stamp.MaybeSetAt(cursor, _tileToSet);
            }
        }

        private bool IsFloor(Stamp stamp, Vector2Int pt) {
            return stamp.At(pt) == _tileToSet;
        }
    }
}