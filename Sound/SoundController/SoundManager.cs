using Godot;
using System;
using System.Collections.Generic;

public partial class SoundManager : Node {
	[Export] private AudioStreamPlayer backgroundMusicPlayer;
	[Export] private AudioStreamPlayer soundEffectsPlayer;

	public static SoundManager Instance;

	public AudioPath currentAudio { get; private set; } = AudioPath.NONE;

	public override void _Ready() {
		Instance = this;
		backgroundMusicPlayer.Playing = true;
		PlayBackgroundMusic(AudioPath.M_MAIN_THEME);
	}

	private static AudioStream GetAudioStream(AudioPath audioPath) {
		if (audioPath == AudioPath.NONE) {
			return null;
		}

		return (AudioStream)GD.Load(AudioPaths.Lookup.GetValueOrDefault(audioPath));
	}

	public void PlayBackgroundMusic(AudioPath audioPath, float fadeOutDuration = 1f, float fadeInDuration = 1f, float startFromSeconds = 0f) {
		if (audioPath != currentAudio) {
			Tween tween = GetTree().CreateTween();

			if (backgroundMusicPlayer.Playing) {
				tween.TweenProperty(backgroundMusicPlayer, "volume_db", -25f, fadeOutDuration);
			}

			tween.TweenCallback(Callable.From(() => {
				backgroundMusicPlayer.Stream = GetAudioStream(audioPath);
				backgroundMusicPlayer.VolumeDb = -25f;
				backgroundMusicPlayer.Play(startFromSeconds);
			}));

			tween.TweenProperty(backgroundMusicPlayer, "volume_db", 0f, fadeInDuration);
			currentAudio = audioPath;
		}
	}

	public void PauseBackgroundMusic() {
		currentAudio = AudioPath.NONE;
		backgroundMusicPlayer.Stop();
	}

	public void PlaySoundEffect(AudioPath audioPath) {
		soundEffectsPlayer.Stream = GetAudioStream(audioPath);
		soundEffectsPlayer.Play();
	}

	public void SetMusicVolume(float volumeDb) {
		backgroundMusicPlayer.VolumeDb = volumeDb;
	}

	public float GetCurrentPlaybackSeconds() {
		return this.backgroundMusicPlayer.GetPlaybackPosition();
	}
}
