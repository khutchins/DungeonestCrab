using Pomerandomian;
using System.Collections.Generic;
using UnityEngine;
using KH.Noise;

namespace DungeonestCrab.Dungeon.Generator {
    /// <summary>
    /// Stams multiple layers of actions based on noise "bands" or thresholds.
    /// Useful for creating topography (hills, beaches) from a single noise source.
    /// </summary>
    public class BandStamper : IAlterer {

        public struct BandEntry {
            public float Min;
            public float Max;
            public ITileAction Action;
        }

        readonly INoiseSource _noise;
        readonly INoiseModifier _modifier;
        readonly List<BandEntry> _bands;
        readonly Bounds _bounds;

        public BandStamper(INoiseSource noise, INoiseModifier modifier, List<BandEntry> bands, Bounds bounds) {
            _noise = noise;
            _modifier = modifier;
            _bands = bands;
            _bounds = bounds;
        }

        public bool Modify(TheDungeon dungeon, IRandom rand) {
            Bounds actualBounds = _bounds ?? new FullBounds();
            AppliedBounds applied = actualBounds.Apply(dungeon.Bounds);
            
            _noise.SetSeed(rand.Next(int.MaxValue));

            AppliedBounds samplingBounds = new AppliedBounds(0, 0, applied.w - 2, applied.h - 2);

            for (int iy = 1; iy < applied.h - 1; iy++) {
                for (int ix = 1; ix < applied.w - 1; ix++) {
                    Vector2Int dungeonPt = new Vector2Int(ix + applied.x, iy + applied.y);
                    TileSpec spec = dungeon.GetTileSpecSafe(dungeonPt);
                    if (spec == null || spec.Immutable) continue;

                    float val = _noise.At(ix, iy);
                    if (_modifier != null) {
                        val -= _modifier.Modifier(samplingBounds, ix - 1, iy - 1);
                    }

                    // Apply all actions whose bands cover this noise value
                    foreach (var band in _bands) {
                        if (val >= band.Min && val < band.Max && band.Action != null) {
                            band.Action.Apply(spec, rand);
                        }
                    }
                }
            }

            return true;
        }
    }
}
