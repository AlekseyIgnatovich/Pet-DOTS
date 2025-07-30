using Unity.Entities;
using Fusion;
using Unity.Transforms;

public class PlayerInputBridge : NetworkBehaviour
{
    private Entity _entity;
    private EntityManager _entityManager;

    public override void Spawned()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var playerArchetype = _entityManager.CreateArchetype(typeof(SquadData), typeof(SquadMoveInput), typeof(SquadCameraTarget), typeof(LocalTransform), typeof(DebugSphere));
        _entity = _entityManager.CreateEntity(playerArchetype);
        _entityManager.AddComponentData(_entity, new SquadData() { MoveSpeed = 2, RotationSpeed = 4, RowCount = 4, StartUnitsCount = 100 });
        _entityManager.AddComponent<SquadSpawnTag>(_entity);
    }

    public override void FixedUpdateNetwork()
    {
        if (_entityManager.Exists(_entity))
        {
            _entityManager.AddComponentData(_entity, new LocalTransform { Position = transform.localPosition, Rotation = transform.localRotation, Scale = 1 });
        }
    }
}