using Godot;
using System;

public partial class phys_hit : Area3D
{
    [Export]
    private float force = 10;
    [Export]
    private float instTime = .15f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        float deltaF = (float)delta;
        instTime -= deltaF;
        if (instTime <= 0)
        {
            this.QueueFree(); // destroys self after instTime seconds
        }
    }

    private void _on_area_3d_body_entered(train_ball obj)
    {
        if (obj is train_ball)
        {
            //	launch the train in a direction with calculations and shit
            Vector3 velocity = new Vector3(0, 0, 0);
            Vector3 objPos = obj.getGlobalLoc();
            velocity = this.GlobalPosition - objPos;
            velocity = velocity.Normalized();


            // call function in train_ball, giving in a vector3 direction+magnitude
            obj.Launch(velocity);
            this.QueueFree(); // destroys the phys_hit on contact with ILaunchable object
        }
    }

    // hopefully THIS ONE works
    private void _on_body_entered(Node3D obj)
    {
        train_ball reference = obj as train_ball;

        if (reference != null)
        {
            GD.Print("ye");
            //	launch the train in a direction with calculations and shit
            Vector3 velocity = new Vector3(0, 0, 0);
            Vector3 objPos = reference.getGlobalLoc();
            velocity = objPos - this.GlobalPosition;
            velocity = velocity.Normalized();


            // call function in train_ball, giving in a vector3 direction+magnitude
            reference.Launch(velocity * force);
            this.QueueFree(); // destroys the phys_hit on contact with ILaunchable object
        }
        else
        {
            GD.Print("nah");
        }
    }
}
