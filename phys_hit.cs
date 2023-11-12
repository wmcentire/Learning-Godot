using Godot;
using System;

public partial class phys_hit : Area3D
{
    [Export]
    private float force = 10;
    private float instTime = .15f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("created pulse");
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
}
