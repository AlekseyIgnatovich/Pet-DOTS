using Unity.Entities;
using UnityEngine;

public struct SpawnerData : IComponentData
{
    public int SpawnCount;
    public Entity Prefab;
    // public GameObject SpawnPrefab;
}
