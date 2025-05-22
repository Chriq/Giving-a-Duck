using Godot;
using System;

public partial class PlayerController : CharacterBody2D {

    public PlayerInfo info;

    [Export] float speed = 120;
    [Export] float jumpSpeed = -180;
    [Export] float gravity = 400;
    [Export] float friction = 0.2f;
    [Export] float acceleration = 0.5f;

    [Export] MultiplayerSynchronizer sync;

    public override void _Ready() {
        sync.SetMultiplayerAuthority(info.id);
    }


    public override void _PhysicsProcess(double delta) {
        if (sync.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()) {
            Move((float)delta);
        }
    }

    private void Move(float delta) {
        Velocity += new Vector2(0f, gravity * delta);

        float dir = Input.GetAxis("Left", "Right");

        if (dir != 0) {
            Velocity = new Vector2(Mathf.Lerp(Velocity.X, dir * speed, acceleration), Velocity.Y);
        } else {
            Velocity = new Vector2(Mathf.Lerp(Velocity.X, 0f, friction), Velocity.Y);
        }

        MoveAndSlide();

        if (Input.IsActionJustPressed("Jump") && IsOnFloor()) {
            Velocity = new Vector2(Velocity.X, jumpSpeed);
        }
    }
}
