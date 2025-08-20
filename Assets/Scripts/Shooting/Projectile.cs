using Unity.Entities;
using Unity.Mathematics;

public struct Projectile : IComponentData
{
    public float Speed;
    public float3 Direction;
}

public struct Lifetime : IComponentData
{
    public float Value;
}