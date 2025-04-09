using Unity.Entities;
using UnityEngine;

class PlayerAuthoringBaker : MonoBehaviour
{
    [SerializeField] public float _speed;
}

class PlayerAuthoringBakerBaker : Baker<PlayerAuthoringBaker>
{
    public override void Bake(PlayerAuthoringBaker authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.Dynamic), new PlayerSpeedData()
        {
            Value = authoring._speed
        });
    }
}
