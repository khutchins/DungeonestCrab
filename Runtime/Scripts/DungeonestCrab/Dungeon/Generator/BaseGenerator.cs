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
            ISeededRandom random = GetRandom();
            return generator.Generate(random);
        }

        public virtual ISeededRandom GetRandom() {
			return new Xoshiro256PpRandom();
		}

		public DungeonPrinter Printer {
			get => GetComponent<DungeonPrinter>();
		}

		protected virtual void Awake() {
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