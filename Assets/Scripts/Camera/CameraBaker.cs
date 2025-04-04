using Unity.Entities;
using UnityEngine;

class CameraBaker : MonoBehaviour
{
    
}

class CameraBakerBaker : Baker<CameraBaker>
{
    public override void Bake(CameraBaker authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<CameraSystem.CameraTag>(entity);
        AddComponent<RenderCamera>(entity);
    }
}
