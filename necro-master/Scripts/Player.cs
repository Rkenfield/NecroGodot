using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] public int Speed { get; set; } = 50;
    [Export] public int FallAcceleration { get; set; } = 75;
    [Export] public Camera3D Camera { get; set; }
    [Export] public CharacterBody3D Cursor { get; set; }
    
    private Vector3 _targetVelocity = Vector3.Zero;

    private float _reticalDistBeforeFollow;
    public override void _Ready()
    {
        base._Ready();
        if (Cursor != null)
        {
            if (Cursor is Reticle reticle)
            {
               reticle.SetPlayer(this);
               _reticalDistBeforeFollow = reticle.MaxReticleDistance;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        CheckMovementInput((float)delta);
        CheckAbilityInput();
    }

    private void CheckAbilityInput()
    {
        
        if (Input.IsActionJustPressed("Gather"))
        {
            Vector2 mousePos = GetViewport().GetMousePosition();

            Vector3 rayOrigin = Camera.ProjectRayOrigin(mousePos);
            Vector3 rayDirection = Camera.ProjectRayNormal(mousePos);
            Vector3 rayEnd = rayOrigin + rayDirection * 1000;
            PhysicsDirectSpaceState3D spaceState3D = PhysicsServer3D.SpaceGetDirectState(GetWorld3D().Space);
            var query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);
            var result = spaceState3D.IntersectRay(query);
            if (result.Count > 0)
            {
                Vector3 hitPos = (Vector3) result["position"];
                GD.Print($"Hit position: {hitPos}");
            }
            else
            {
                GD.Print("Nothing Hit");
            }
        }
        if (Input.IsActionJustPressed("Send"))
        {
            
        }
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

        if (direction.Length() <= 0)
        {
            var axisX = Input.GetJoyAxis(0, JoyAxis.LeftX);
            var axisY = Input.GetJoyAxis(0, JoyAxis.LeftY);
            //GD.Print("X: " + axisX + " Y: " + axisY);
            direction = new Vector3(-axisX, 0, -axisY);
            
            if (direction.Length() < .05)
            {
                direction = Vector3.Zero;
            }
        }
        else
        {
            //direction = direction.Normalized();
        }
        HandleMovement(direction,delta);
    }

    private void HandleMovement(Vector3 direction, float delta)
    {
        if (Cursor == null)
        {
            return;
        }
        if (direction != Vector3.Zero)
        {
            
            //GD.Print("Direction: " + direction);
            var aimDir = GlobalPosition.DirectionTo(Cursor.GlobalPosition);
            if (aimDir.Length() > 0)
            {
                GetNode<Node3D>("CharacterMesh").Basis = Basis.LookingAt(new Vector3(aimDir.X,0,aimDir.Z),null,true);
            }
            
            var aim = Camera.Basis;
            var forward = (aim.Z).Normalized();
            var right = aim.X.Normalized();
            _targetVelocity = ((forward * direction.Z) + (right * direction.X)) * (2*Speed * delta);
            //GD.Print("Target: " + _targetVelocity);
        
            if (Cursor is Reticle reticle)
            {
                //GD.Print("Made it to reticle cast");
                reticle.SetVelocity(_targetVelocity); 
            }
            
            if (!IsOnFloor())
            {
                _targetVelocity.Y -= FallAcceleration * delta;
            }
            
             
            Vector3 start = new Vector3(GlobalPosition.X, 0, GlobalPosition.Z);
            Vector3 target = new Vector3(Cursor.GlobalPosition.X, 0, Cursor.GlobalPosition.Z);
            float dist = start.DistanceTo(target);
            GD.Print("Dist to puck: " + dist);
            if (dist >= _reticalDistBeforeFollow*0.8)
            {
                Velocity = start.DirectionTo(target) * Speed * delta;
                GD.Print("Vel: " +Velocity);
                MoveAndSlide();
            }
            
            //Velocity = _targetVelocity;
            //Call to trigger the movement
            //
        }
    }
}
