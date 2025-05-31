using Godot;
using System;

public partial class MultiplayerMenu : Node {
    [Export] TextEdit nameInput;

    [Export] TextEdit ipInput;
    [Export] TextEdit portInput;

    private void Host() {
        MultiplayerController.Instance.Host();
    }

    private void Join() {
        MultiplayerController.Instance.Join();
    }
}