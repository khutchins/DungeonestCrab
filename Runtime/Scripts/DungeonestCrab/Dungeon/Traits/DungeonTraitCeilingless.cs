using UnityEngine;

namespace DungeonestCrab.Dungeon.Printer {
    [CreateAssetMenu(menuName = "Dungeon/Traits/Ceilingless")]
    public class TraitCeilingless : DungeonTraitSO {
        public override void ModifyTileRules(TileSpec spec, ref TileRuleConfig config) {
            config.DrawCeiling = false;
        }
    }
}