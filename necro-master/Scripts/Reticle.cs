using Godot;
using System;

public partial class Reticle : CharacterBody3D
{
    [Export] public float MaxReticleDistance { get; set; } =50;
    private Player _owningPlayer;

    public void SetPlayer(Player inPlayer)
    {
        _owningPlayer = inPlayer;
    }
    public void SetVelocity(Vector3 velocity)
    {
       
       
        /*if (dist >= MaxReticleDistance)
        {
            Vector3 dir = ((target+velocity) - start).Normalized();
            var clampedLoc = dir * MaxReticleDistance;
            var newVel = clampedLoc - target;
            Velocity = newVel;
           
        }
        else
        {
            Velocity = velocity;
        }*/
        Velocity = velocity;
        MoveAndSlide();
        Vector3 start = new Vector3(_owningPlayer.GlobalPosition.X, 0, _owningPlayer.GlobalPosition.Z);
        Vector3 target = new Vector3(GlobalPosition.X, 0, GlobalPosition.Z);
        velocity = new Vector3(velocity.X, 0, velocity.Z);
        float dist =  start.DistanceTo(target);
        GD.Print(dist);
        if (dist > MaxReticleDistance)
        {
            GlobalPosition = start + start.DirectionTo(target) * MaxReticleDistance;
        }
    }
}
