using Godot;
using System;

public partial class train_ball : RigidBody3D , ILaunchable
{
    [Export]
    private NodePath _path;
    [Export]
    private string lastTagged;
    [Export]
    private float minDmgVel = 15f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

    // when it hits a player
    public void _on_body_entered(Node node)
    {
        bool isPlayerCol = false;
        // check the velocity of self
        if (this.LinearVelocity.Length() > minDmgVel)
        {
            // check to see if it hit the player
            CharacterBody3D player = node as CharacterBody3D;
            if(player != null)
            {
                int damage = 0;
                // Damage calculations
                damage = (int)(LinearVelocity.Length());
                player.playerDamage(damage , lastTagged);       
                isPlayerCol = true;
            }
            if (!isPlayerCol)
            {
                // if a train hits another train and that train kills a player,
                // still get the credit by TAG TRANSFERS!
                train_ball tb = node as train_ball;
                if (tb != null)
                {
                    tb.lastTagged = lastTagged;
                    //GD.Print("Transfered tag " + lastTagged);
                }
            }
        }
        
        
    }

    public void _on_body_shape_entered(Rid body_rid, Node body, int body_shape_index, int local_shape_index)
    {
        //GD.Print("COLLISION");
        // IGNOR THIS FUNCTION IT IS MEANING LESS
    }

    public void Launch(Vector3 velocity, string id)
    {
        //GD.Print("LLAUNCHED BY: " + id);
        lastTagged = id;
        ApplyCentralImpulse(velocity);
    }
    /// <summary>
    /// this is just so the ILaunchable actually works in my logic, ignore please
    /// </summary>
    /// <returns></returns>
    public Vector3 getGlobalLoc()
    {
        return GlobalPosition;
    }

    // for some reason the rigidbody collision signals are not working,
    // so I'm going to be using this instead for now.
    // IT TURNS OUT CONTACT MONITER IS OFF BY DEFAULT SO THAT'S FIXED NOW
    // IGNORE THE COMMENTS ABOUT IT NOT WORKING

    
}
