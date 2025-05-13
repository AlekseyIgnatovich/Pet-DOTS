using Unity.Entities;
using UnityEngine;

public class SpawnerData : IComponentData
{
    public int SpawnCount;
    public Entity Prefab;
    public GameObject SpawnPrefab;
}
