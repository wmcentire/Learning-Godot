using Godot;
using System;

public partial class train_ball : Node3D , ILaunchable
{
    [Export]
    private NodePath _path;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void Launch(Vector3 velocity)
    {
        RigidBody3D rb = GetNode<RigidBody3D>(_path);
        rb.ApplyCentralImpulse(velocity);
    }
    /// <summary>
    /// this is just so the ILaunchable actually works in my logic, ignore please
    /// </summary>
    /// <returns></returns>
    public Vector3 getGlobalLoc()
    {
        return GlobalPosition;
    }
}
