using DungeonestCrab.Dungeon;
using KH.Noise;
using Pomerandomian;
using UnityEngine;
using XNode;
using static XNode.Node;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    public abstract class SourceProviderNode : BasePreviewNode {
        [Output(ShowBackingValue.Never, ConnectionType.Multiple)] public SourceConnection Output;

        public abstract ISource GetSource();

        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") return GetSource();
            return null;
        }

        public Texture2D GetPreviewTexture() => PreviewTexture;

        public override void UpdatePreview() {
            int w = 64; int h = 64;
            ValidatePreviewTexture(w, h);

            Stamp stamp = new Stamp(w, h);
            ISource source = GetSource();
            if (source != null) source.Generate(stamp, new SystemRandom(12345));

            Color[] cols = new Color[w * h];
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    Tile t = stamp.At(x, y);
                    if (t == Tile.Wall) cols[y * w + x] = Color.black;
                    else if (t == Tile.Floor) cols[y * w + x] = Color.white;
                    else cols[y * w + x] = new Color(0.25f, 0.25f, 0.25f);
                }
            }
            PreviewTexture.SetPixels(cols);
            PreviewTexture.Apply();
        }
    }


    [CreateNodeMenu("Dungeon/Sources/All")]
    public class SourceAllNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        public override ISource GetSource() => new SourceAll(TileToSet);
    }

    [CreateNodeMenu("Dungeon/Sources/Maze")]
    public class SourceMazeNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        [Range(0, 1)] public float StraightBias = 0.5f;
        [Range(0, 1)] public float BraidPercent = 0f;
        public bool Conservative = false;

        public override ISource GetSource() => new SourceMaze(TileToSet, StraightBias, Conservative, BraidPercent);
    }

    [CreateNodeMenu("Dungeon/Sources/Maze (Haphazard)")]
    public class SourceMazeHaphazardNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        public int MaxMoveDist = 3;
        public float InsertionAttempts = 2f;
        public int MoveMultplier = 1;

        public override ISource GetSource() => new SourceMazeHaphazard(TileToSet, MaxMoveDist, InsertionAttempts, MoveMultplier);
    }

    [CreateNodeMenu("Dungeon/Sources/Cave")]
    public class SourceCaveNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        [Range(0, 1)] public float InitialOdds = 0.5f;
        public int MinNeighbors = 3;
        public int MaxNeighbors = 8;
        public int Iterations = 3;
        public bool Invert = false;

        public override ISource GetSource() => new SourceCave(TileToSet, InitialOdds, MinNeighbors, MaxNeighbors, Invert, Iterations);
    }

    [CreateNodeMenu("Dungeon/Sources/River")]
    public class SourceRiverNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        public float MinWidth = 0.5f;
        public float MaxWidth = 1.5f;
        public SourceRiver.Sides StartSides = SourceRiver.Sides.Top;
        public SourceRiver.Sides EndSides = SourceRiver.Sides.Bottom;

        public override ISource GetSource() => new SourceRiver(TileToSet, MinWidth, MaxWidth, StartSides, EndSides);
    }

    [CreateNodeMenu("Dungeon/Sources/Stripes")]
    public class SourceStripeNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        [Range(0, 1)] public float Density = 0.2f;
        [Range(0, 1)] public float CurveOdds = 0.1f;
        [Range(0, 1)] public float DeadEndOdds = 0.1f;

        public override ISource GetSource() => new SourceStripe(TileToSet, Density, CurveOdds, DeadEndOdds);
    }

    [CreateNodeMenu("Dungeon/Sources/Voronoi")]
    public class SourceVoronoiNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        public int Cells = 5;
        public int RegionSize = 4;
        public int Iterations = 3;

        public override ISource GetSource() => new SourceVoronoi(TileToSet, Cells, RegionSize, Iterations);
    }

    [CreateNodeMenu("Dungeon/Sources/Circle")]
    public class SourceCircleNode : SourceProviderNode {
        public Tile TileToSet = Tile.Floor;
        public override ISource GetSource() => new SourceCircle(TileToSet);
    }

    [CreateNodeMenu("Dungeon/Sources/Noise")]
    public class SourceNoiseNode : SourceProviderNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public NoiseConnection NoiseInput;
        public Tile TileToSet = Tile.Floor;
        [Range(0, 1)] public float Threshold = 0.5f;

        public override ISource GetSource() {
            INoiseSource noise = GetInputValue<INoiseSource>("NoiseInput", null);
            // Default noise if not connected
            if (noise == null) noise = new NoiseSourcePerlin(0.1f);
            return new SourceNoise(TileToSet, Threshold, noise, null);
        }
    }
}