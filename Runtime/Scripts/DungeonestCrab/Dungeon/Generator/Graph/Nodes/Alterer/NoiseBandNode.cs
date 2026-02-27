using UnityEngine;
using XNode;

namespace DungeonestCrab.Dungeon.Generator.Graph {

    [CreateNodeMenu("Dungeon/Actions/Noise Band")]
    public class NoiseBandNode : Node {
        [Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public ActionConnection Action;
        [Output(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)] public NoiseBandConnection Output;

        [Range(0, 1)] public float MinThreshold = 0f;
        [Range(0, 1)] public float MaxThreshold = 1f;

        public override object GetValue(NodePort port) {
            if (port.fieldName == "Output") {
                return new BandStamper.BandEntry {
                    Min = MinThreshold,
                    Max = MaxThreshold,
                    Action = GetInputValue<ITileAction>("Action", null)
                };
            }
            return null;
        }
    }
}
