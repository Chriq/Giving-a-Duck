using Godot;
using System;

public partial class AirState : State {
    [Export] float speed = 60;
    [Export] float friction = 0.7f;
    [Export] float acceleration = 0.1f;

    public override void Enter() {
        animator.Play("fly");
    }



    public override void PhysicsDo(double delta) {
        float dir = Input.GetAxis("Left", "Right");

        if (dir != 0) {
            rb.Velocity = new Vector2(Mathf.Lerp(rb.Velocity.X, dir * speed, acceleration), rb.Velocity.Y);
        } else {
            rb.Velocity = new Vector2(Mathf.Lerp(rb.Velocity.X, 0f, friction), rb.Velocity.Y);
        }

        if (rb.IsOnFloor()) {
            complete = true;
        }
    }
}
