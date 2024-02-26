using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Crawl {
	public enum InputType {
		Unknown = 0,
		Mouse = 1,
		Keyboard = 2,
		Controller = 3
	}

	public enum VibrateMode {
		Always = 0,
		Smart = 1,
		Never = 2
	}


	public abstract class CurrentInputSO : ScriptableObject {
		public VibrateMode VibrateMode = VibrateMode.Smart;
		public abstract InputType LastInputType { get; }
		public abstract void Vibrate(int motorIndex, float motorLevel, float duration);
	}
}