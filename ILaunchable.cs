using Godot;
using System;

public interface ILaunchable
{
    public void Launch(Vector3 velocity);
    public Vector3 getGlobalLoc();
}
