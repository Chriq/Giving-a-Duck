using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerController : CharacterBody2D {

    public PlayerInfo info;

    [Export] float speed = 120;
    [Export] float jumpSpeed = -180;
    [Export] float gravity = 400;
    [Export] float friction = 0.2f;
    [Export] float acceleration = 0.5f;

    [Export] MultiplayerSynchronizer sync;

    /** State Machine **/
    private StateMachine machine;
    [Export] RunState runState;
    [Export] IdleState idleState;
    [Export] AirState airState;
    [Export] WallState wallState;

    private List<Item> items = new();
    private int jumps = 0;

    public override void _Ready() {
        if (info != null) sync.SetMultiplayerAuthority(info.id);

        idleState.Setup(this, null);
        runState.Setup(this, null);
        airState.Setup(this, null);
        wallState.Setup(this, null);

        machine = new();
        machine.Set(idleState);
    }

    public void SelectState(float axis) {
        if (IsOnFloor()) {
            if (axis == 0) {
                machine.Set(idleState);
            } else {
                machine.Set(runState);
            }
        } else if (IsOnWall() && items.Contains(Item.WALL_JUMP)) {
            machine.Set(wallState);
        } else {
            machine.Set(airState);
        }
    }


    public override void _PhysicsProcess(double delta) {
        if (sync.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()) {
            HandleJump();
            Move((float)delta);
        }
    }

    private void Move(float delta) {
        Velocity += new Vector2(0f, gravity * delta);

        float input = Input.GetAxis("Left", "Right");
        SelectState(input);
        machine.state.PhysicsDo(delta);

        MoveAndSlide();
    }

    private void HandleJump() {
        if (Input.IsActionJustPressed("Jump")) {
            if (IsOnFloor()) {
                jumps = 0;
            }

            // if (items.Contains(Item.WALL_JUMP) && IsOnWall()) {
            //     jumps = 0;
            // }

            if (jumps < 1 || items.Contains(Item.DOUBLE_JUMP) && jumps < 2) {
                Velocity = new Vector2(Velocity.X, jumpSpeed);
                jumps++;
            }
        }
    }

    public void Give(Item item) {
        items.Add(item);
    }
}
