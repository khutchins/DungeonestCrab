using DungeonestCrab.Dungeon.Printer;
using Pomerandomian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Generator {
    /// <summary>
    /// Example base generator for dungeon generation.
    /// </summary>
    [RequireComponent(typeof(DungeonPrinter))]
    public abstract class BaseGenerator : MonoBehaviour {
		[SerializeField] bool GenerateOnAwake = true;

		public virtual TheGenerator CreateGenerator() { return null; }

        protected virtual TheDungeon Make() {
            TheGenerator generator = CreateGenerator();
            IRandom random = GetRandom();
            return generator.Generate(random);
        }

        public virtual IRandom GetRandom() {
			return new SystemRandom();
		}

		public DungeonPrinter Printer {
			get => GetComponent<DungeonPrinter>();
		}

		private void Awake() {
			if (GenerateOnAwake) Print();
		}

		public void Print() {
			Printer.Print(Make());
		}

		public void PrintTest() {
			Printer.PrintForTest(Make());
		}

		public void ClearTest() {
			Printer.ClearGeneratedTestDungeon();
		}
    }
}