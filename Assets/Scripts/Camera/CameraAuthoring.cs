using Unity.Entities;
using UnityEngine;

class CameraAuthoring : MonoBehaviour
{
    
}

class CameraAuthoringBaker : Baker<CameraAuthoring>
{
    public override void Bake(CameraAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<RenderCamera>(entity);
        AddComponent<CameraFollowTag>(entity);
    }
}
