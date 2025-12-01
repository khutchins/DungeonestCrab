using DungeonestCrab.Dungeon.Generator.Graph;
using Pomerandomian;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    public class TestGenerator : BaseGenerator {
        public DungeonGraph DungeonToGenerate;

        [Header("Testing")]
        public bool RandomizeSeed = false;
        [DisableIf("RandomizeSeed")]
        public int Seed = 313;

        public override IRandom GetRandom() {
            if (RandomizeSeed) {
                Seed = new SystemRandom().Next(1000000);
            }
            return new SystemRandom(Seed);
        }

        protected override TheDungeon Make() {
            IRandom rand = GetRandom();

            if (DungeonToGenerate != null) {
                return DungeonToGenerate.Generate(rand);
            } else {
                Debug.LogWarning("No generator assigned.");
                return null;
            }
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