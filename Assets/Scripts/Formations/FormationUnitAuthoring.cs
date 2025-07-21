using Unity.Entities;
using UnityEngine;

public class FormationUnitAuthoring : MonoBehaviour
{
    public int Index;
}

public class FormationUnitBaker : Baker<FormationUnitAuthoring>
{
    public override void Bake(FormationUnitAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new FormationUnit { Index = authoring.Index });
        AddComponent<UnitTargetPosition>(entity); // Начальная цель — та же позиция
    }
}
