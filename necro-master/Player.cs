using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] public int Speed { get; set; } = 50;
    [Export] public int FallAcceleration { get; set; } = 75;
    
    private Vector3 _targetVelocity = Vector3.Zero;

    public override void _PhysicsProcess(double delta)
    {
        CheckMovementInput((float)delta);
    }

    private void CheckMovementInput(float delta)
    {
        var direction = Vector3.Zero;
        if (Input.IsActionPressed("MoveForward"))
        {
            direction.Z += 1;
        }
        if (Input.IsActionPressed("MoveBackward"))
        {
            direction.Z -= 1;
        }
        if (Input.IsActionPressed("MoveRight"))
        {
            direction.X -= 1;
        }
        if (Input.IsActionPressed("MoveLeft"))
        {
            direction.X += 1;
        }
        HandleMovement(direction,delta);
    }

    private void HandleMovement(Vector3 direction, float delta)
    {
        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
            GetNode<Node3D>("CharacterMesh").Basis = Basis.LookingAt(direction,null,true);
            
            var aim = GetNode<Node3D>("CharacterMesh").Basis;
            var forward = (aim.Z).Normalized();
            _targetVelocity = forward * Speed;
            
            if (!IsOnFloor())
            {
                _targetVelocity.Y -= FallAcceleration * delta;
            }
            Velocity = _targetVelocity;
            //Call to trigger the movement
            MoveAndSlide();
        }
    }
}
