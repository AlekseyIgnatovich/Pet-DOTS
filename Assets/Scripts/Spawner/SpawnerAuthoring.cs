using Unity.Entities;
using UnityEngine;

class SpawnerAuthoring : MonoBehaviour
{
    public int count = 100;
    public GameObject unitPrefab;
    public GameObject projectilePrefab;
}

class SpawnerAuthoringBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        var entityUnitPrefab = GetEntity(authoring.unitPrefab, TransformUsageFlags.Dynamic);
        var entityProjectilePrefab = GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic);
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new SpawnerData()
        {
            UnitPrefab = entityUnitPrefab,
            ProjectilePrefab = entityProjectilePrefab,
        });
    }
}
