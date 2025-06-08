using Godot;
using System;

public partial class WallState : State {
    [Export] float friction = 0.2f;
    [Export] float jumpSpeed = 400;

    [Export] float speed = 20;
    [Export] float acceleration = 0.05f;

    [Export] float climbSpeed = 50;

    private double frameBuffer = 0;

    private double timer = 0d;

    public override void Enter() {
        animator.Play("idle");
        timer = 0;
    }


    public override void PhysicsDo(double delta) {
        float dir = Input.GetAxis("Left", "Right");

        // make buffer for players to let go of wall without jumping
        if (dir != 0) {
            if (frameBuffer < 0.2d) {
                frameBuffer += delta;
            } else {
                rb.Velocity = new Vector2(Mathf.Lerp(rb.Velocity.X, dir * speed, acceleration), rb.Velocity.Y);
                frameBuffer = 0;
                complete = true;
            }
        } else {
            frameBuffer = 0;
        }

        float xStickToWall = rb.Velocity.X + rb.GetWallNormal().X * -1f;
        float yFrictionGravity = Mathf.Lerp(rb.Velocity.Y, 0f, friction);
        rb.Velocity = new Vector2(xStickToWall, yFrictionGravity);
        if (Input.IsActionJustPressed("Jump") && rb.HasItem(Item.WALL_JUMP)) {
            WallJump();
            complete = true;
        }

        if (Input.IsActionPressed("Up") && rb.HasItem(Item.CLIMB) && timer < 2) {
            timer += delta;
            Climb();
        }
    }

    private void WallJump() {
        Vector2 jumpForce = rb.GetWallNormal() + (Vector2.Up * 0.5f);
        rb.Velocity += jumpForce.Normalized() * jumpSpeed;
    }

    private void Climb() {
        rb.Velocity = new Vector2(rb.Velocity.X, -climbSpeed);
    }

}
