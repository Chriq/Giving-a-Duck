using Godot;
using System;

public partial class MultiplayerMenu : VBoxContainer {
    private void Host() {
        MultiplayerController.Instance.Host();
    }

    private void Join() {
        MultiplayerController.Instance.Join();
    }

    private void Start() {
        MultiplayerController.Instance.Start();
    }

    private void Close() {
        Hide();
    }
}