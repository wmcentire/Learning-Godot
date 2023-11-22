using Godot;
using System;

public interface IWeapon
{
    public void shoot();
    public bool getShoot();
    public void setShoot(bool shoot);
    public void setPID(string pID);
}
