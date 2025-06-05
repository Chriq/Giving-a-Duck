using Godot;
using System;

public partial class Beacon : Area2D {
    bool discovered = false;

    public override void _Ready() {
        discovered = GameManager.Instance.discoveredBeacons.Contains(GetPath());
        BodyEntered += Discover;
    }

    public void Discover(Node n) {
        if (discovered) {
            GD.Print("Already discovered");
        }

        if (!discovered && n is PlayerController) {
            discovered = true;
            GameManager.Instance.discoveredBeacons.Add(GetPath());
            GameManager.Instance.CheckBeacons();
        }


    }

}
