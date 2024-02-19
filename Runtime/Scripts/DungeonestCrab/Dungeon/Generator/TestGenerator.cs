using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    public class TestGenerator : BaseGenerator {
        [SerializeField] DungeonSpecSO DungeonToGenerate;

        [Header("Testing")]
        public bool RandomizeSeed = false;
        [DisableIf("RandomizeSeed")]
        public int Seed = 313;

        public override TheGenerator CreateGenerator() {
            return new TheGenerator(DungeonToGenerate.ToDungeonSpec());
        }

        [Button("Generate Test Dungeon")]
        void TestGenerate() {
            if (DungeonToGenerate != null) {
                PrintTest();
            }
        }

        [Button("Clear Test Dungeon")]
        void ClearGenerate() {
            ClearTest();
        }
    }
}