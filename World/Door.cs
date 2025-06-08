using Godot;
using System;

public partial class Door : Area2D {
    [Export] AnimatedSprite2D sprite;

    private bool open = false;

    public override void _Ready() {
        GameManager.Instance.AllBeaconsFound += OpenDoor;
        if (GameManager.Instance.discoveredBeacons.Count == Consts.NUM_TOTAL_BEACONS) {
            OpenDoor();
        }

        BodyEntered += OnEnter;
    }

    private void OpenDoor() {
        GameManager.Instance.AllBeaconsFound -= OpenDoor;
        sprite.Frame = 1;
        open = true;
    }

    private void OnEnter(Node n) {
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
