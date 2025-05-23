using Godot;
using System;

public partial class WallState : State {
    [Export] float friction = 0.2f;
    [Export] float jumpSpeed = 400;

    public override void PhysicsDo(double delta) {
        rb.Velocity = new Vector2(rb.Velocity.X, Mathf.Lerp(rb.Velocity.Y, 0f, friction));
        if (Input.IsActionJustPressed("Jump")) {
            WallJump();
        }
    }

    private void WallJump() {
        Vector2 jumpForce = rb.GetWallNormal() + (Vector2.Up * 0.5f);
        rb.Velocity += jumpForce.Normalized() * jumpSpeed;
    }

}
