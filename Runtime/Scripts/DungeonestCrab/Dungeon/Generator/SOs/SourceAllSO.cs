using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    [CreateAssetMenu(menuName = "DungeonestCrab/Spec/Source - All")]
    [InlineEditor]
    public class SourceAllSO : SourceSO {
        public override ISource ToSource() {
            return new SourceAll(TileToSet);
        }
    }
}