using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerController : CharacterBody2D {
    public int playerId;
    public PlayerInfo info => GameManager.Instance.players.GetValueOrDefault(playerId);

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
    [Export] DashState dashState;

    [Export] bool dev = false;

    private int jumps = 0;

    public override void _Ready() {
        sync.SetMultiplayerAuthority(playerId);

        idleState.Setup(this, null);
        runState.Setup(this, null);
        airState.Setup(this, null);
        wallState.Setup(this, null);
        dashState.Setup(this, null);

        machine = new();
        machine.Set(idleState);

        if (dev) {
            playerId = 1;
            PlayerInfo devInfo = new(1, "test");
            List<Item> allItemList = ((Item[])Enum.GetValues(typeof(Item))).ToList();
            devInfo.items.AddRange(allItemList);
            GameManager.Instance.players.Add(1, devInfo);
        }

        CallDeferred(MethodName.InitMap);
        foreach (int i in MapManager.Instance.GetChucksToLoad(GlobalPosition)) {
            GD.Print(i, ", ");
        }

    }

    private void InitMap() {
        MapManager.Instance.UpdateActive(GlobalPosition);
    }

    public void SelectState(float axis) {
        if (Input.IsActionJustPressed("Dash") || machine.state == dashState) {
            machine.Set(dashState);
        }

        if (IsOnFloor()) {
            if (axis == 0) {
                machine.Set(idleState);
            } else {
                machine.Set(runState);
            }
        } else if (IsOnWall() && info.items.Contains(Item.WALL_JUMP)) {
            machine.Set(wallState);
        } else {
            machine.Set(airState);
        }
    }


    public override void _PhysicsProcess(double delta) {
        if (sync.GetMultiplayerAuthority() == Multiplayer.GetUniqueId() || dev) {
            HandleJump();
            Move((float)delta);
        }

        foreach (int i in MapManager.Instance.GetChucksToLoad(GlobalPosition)) {
            GD.Print(i, ", ");
        }
    }

    private void Move(float delta) {
        Velocity += new Vector2(0f, gravity * delta);

        float input = Input.GetAxis("Left", "Right");
        SelectState(input);
        machine.state.PhysicsDo(delta);

        MoveAndSlide();
    }

    // TODO: fix double + wall jump at same time
    private void HandleJump() {
        if (Input.IsActionJustPressed("Jump")) {
            if (IsOnFloor()) {
                jumps = 0;
            }

            // if (info.items.Contains(Item.WALL_JUMP) && IsOnWall()) {
            //     jumps = 0;
            // }

            if (jumps < 1 || info.items.Contains(Item.DOUBLE_JUMP) && jumps < 2) {
                Velocity = new Vector2(Velocity.X, jumpSpeed);
                jumps++;
            }
        }
    }
}
