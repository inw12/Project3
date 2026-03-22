using UnityEngine;
public struct ProjectileStats
{
    public float Damage;        // amount of damage the projectile will do
    public float Speed;         // how fast the projectile will travel
    public float Range;         // how far the projectile will travel before returning to object pool
    public Vector3 Direction;   // what direction the projectile will travel
}