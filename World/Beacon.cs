using Godot;
using System;

public partial class Beacon : Area2D {
    bool discovered = false;
    [Export] AnimatedSprite2D sprite;
    [Export] PointLight2D light;

    public override void _Ready() {
        discovered = GameManager.Instance.discoveredBeacons.Contains(GetPath());
        BodyEntered += Discover;

        if (discovered) {
            sprite.Frame = 3;
            light.Show();
        }
    }

    public void Discover(Node n) {
        if (!discovered && n is PlayerController) {
            discovered = true;
            sprite.Play("discover");
            sprite.AnimationFinished += () => light.Show();

            GameManager.Instance.UpdateBeacons(GetPath());
            GameManager.Instance.CheckBeacons();
        }
    }
}
