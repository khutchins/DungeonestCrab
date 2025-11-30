using UnityEngine;
using XNode;
using DungeonestCrab.Dungeon;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    public abstract class BaseStamperNode : DungeonPassNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public SourceConnection Source;
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public BoundsConnection Bounds;

        [Tooltip("If checked, the source algorithm will see existing terrain as 'occupied' and may choose to avoid it.")]
        public bool ProtectExistingTerrain = false;
        [Tooltip("If checked, floors won't be overwritten.")]
        public bool PreserveFloors = false;
        [Tooltip("If checked, walls won't be overwritten.")]
        public bool PreserveWalls = false;

        protected abstract IAlterer CreateStamper(ISource source, Bounds bounds);

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            ISource source = GetInputValue<ISource>("Source", null);
            Bounds bounds = GetInputValue<Bounds>("Bounds", null);

            if (source == null) return false;

            IAlterer stamper = CreateStamper(source, bounds);
            return stamper.Modify(dungeon, random);
        }
    }

    [CreateNodeMenu("Dungeon/Actions/Stamp (Shape Only)")]
    public class StampShapeNode : BaseStamperNode {
        protected override IAlterer CreateStamper(ISource source, Bounds bounds) {
            return new Stamper(source, null, ProtectExistingTerrain, bounds, PreserveFloors, PreserveWalls);
        }
    }

    [CreateNodeMenu("Dungeon/Actions/Stamp (With Terrain)")]
    public class StampTerrainNode : BaseStamperNode {
        public TerrainSO Terrain;

        protected override IAlterer CreateStamper(ISource source, Bounds bounds) {
            return new Stamper(source, Terrain, ProtectExistingTerrain, bounds, PreserveFloors, PreserveWalls);
        }
    }

    [CreateNodeMenu("Dungeon/Actions/Stamper")]
    public class StamperNode : DungeonPassNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public SourceConnection Source;
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public BoundsConnection Bounds;

        public TerrainSO Terrain;
        public bool PassTerrains;

        protected override bool ApplyNodeLogic(TheDungeon dungeon, IRandom random) {
            ISource source = GetInputValue<ISource>("Source", null);
            Bounds boundsInput = GetInputValue<Bounds>("Bounds", null);

            if (source == null) return false;
            boundsInput ??= new FullBounds();

            Stamper stamper = new Stamper(source, Terrain, PassTerrains, boundsInput);
            return stamper.Modify(dungeon, random);
        }
    }
}