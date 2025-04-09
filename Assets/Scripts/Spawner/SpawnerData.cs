using Unity.Entities;

public struct SpawnerData : IComponentData
{
    public int SpawnCount;
    public Entity Prefab;
}
