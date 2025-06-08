using Godot;
using System;
using System.Threading.Tasks;

public partial class FadeController : ColorRect {
	public static FadeController Instance;

    public override void _Ready() {
        Instance = this;
		Show();
    }

	public async Task FadeOut(CanvasItem canvas, float duration = 1f) {
		double counter = 0;
		Color og = canvas.Modulate;
		Color startColor = new(og.R, og.G, og.B, 1f);
		Color endColor = new(og.R, og.G, og.B, 0f);

		while(counter < duration) {
			Color lerp = startColor.Lerp(endColor, (float) counter / duration);
			counter += GetProcessDeltaTime();
			canvas.Modulate = lerp;
			await ToSignal(GetTree(), "process_frame");
		}

		canvas.Modulate = endColor;
	}

	public async Task FadeIn(CanvasItem canvas, float duration = 1f) {
		double counter = 0;
		Color og = canvas.Modulate;
		Color startColor = new(og.R, og.G, og.B, 0f);
		Color endColor = new(og.R, og.G, og.B, 1f);

		while(counter < duration) {
			Color lerp = startColor.Lerp(endColor, (float) counter / duration);
			counter += GetProcessDeltaTime();
			canvas.Modulate = lerp;
			await ToSignal(GetTree(), "process_frame");
		}
		
		canvas.Modulate = endColor;
	}
}
