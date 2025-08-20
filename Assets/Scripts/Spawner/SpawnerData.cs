using Unity.Entities;

public struct SpawnerData : IComponentData
{
    public Entity UnitPrefab;
    public Entity ProjectilePrefab;
}
