using Godot;
using System;

public partial class SplashScreen : Node
{
	[Export] private CanvasItem splashScreen;
	[Export] private FadeController fade;
	[Export] private PackedScene scene;

	public override async void _Ready() {
		splashScreen.Modulate = new Color(splashScreen.Modulate, 0f);
		Input.MouseMode = Input.MouseModeEnum.Hidden;

		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		await fade.FadeIn(splashScreen);
		await ToSignal(GetTree().CreateTimer(3f), "timeout");
		await fade.FadeOut(splashScreen);
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		ContinueToGame();
	}

	public void ContinueToGame() {
		GetTree().ChangeSceneToPacked(scene);
	}
}
