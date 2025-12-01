using UnityEngine;
using Pomerandomian;

namespace DungeonestCrab.Dungeon.Generator.Graph {
    public abstract class BaseStamperNode : DungeonPassNode {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public SourceConnection Source;
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public BoundsConnection Bounds;

        [Tooltip("If checked, the source algorithm will see existing terrain as 'occupied' and may choose to avoid it.")]
        public bool ProtectExistingTerrain = true;
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
}