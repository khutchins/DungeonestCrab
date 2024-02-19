namespace KH.Noise {
	/// <summary>
	/// Given an x and y coordinate, returns a number in [0, 1].
	/// </summary>
	public interface INoiseSource {
		void SetSeed(int seed);
		float At(float x, float y);
	}
}