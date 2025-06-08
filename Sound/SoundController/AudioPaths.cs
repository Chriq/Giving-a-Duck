using System.Collections.Generic;

public static class AudioPaths {
	public static Dictionary<AudioPath, string> Lookup = new Dictionary<AudioPath, string>() {
		{AudioPath.M_MAIN_THEME, "res://Sound/PathOfTheDucks.wav"}
	};
}

public enum AudioPath {
	NONE,
	M_MAIN_THEME
}