using Unity.Entities;
using UnityEngine;

class SpawnerBaker : MonoBehaviour
{
    public int count = 100;
    public GameObject prefab;
}

class SpawnerBakerBaker : Baker<SpawnerBaker>
{
    public override void Bake(SpawnerBaker authoring)
    {
        var entityPrefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new SpawnerData() {Prefab = entityPrefab, SpawnCount = authoring.count});
    }
}
