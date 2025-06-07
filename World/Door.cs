using Godot;
using System;

public partial class Door : Area2D {
    [Export] AnimatedSprite2D sprite;

    private bool open = false;

    public override void _Ready() {
        if (GameManager.Instance.discoveredBeacons.Count == Consts.NUM_TOTAL_BEACONS) {
            OpenDoor();
        }

        GameManager.Instance.AllBeaconsFound += OpenDoor;

        BodyEntered += OnEnter;
    }

    private void OpenDoor() {
        sprite.Frame = 1;
        open = true;
    }

    private void OnEnter(Node n) {
        GameManager.Instance.AllBeaconsFound -= OpenDoor;
        GD.Print($"Open: {open} Is Player: {n is PlayerController}");
        if (open && n is PlayerController) {
            CallDeferred(MethodName.ChangeScene);
        }
    }

    private void ChangeScene() {
        GameManager.Instance.EndGame();
        GetTree().ChangeSceneToFile("res://Scenes/Ending.tscn");
    }

}
