using Unity.Entities;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{
    public GameObject unitPrefab;

    class Baker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            var prefabEntity = GetEntity(authoring.unitPrefab, TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitPrefab { Value = prefabEntity });
        }
    }
}

public struct UnitPrefab : IComponentData
{
    public Entity Value;
}