using Fusion;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class NetworkPlayer : NetworkBehaviour
{
    private Entity _entity;
    private EntityManager _entityManager;

    const float rotationSpeed = 30f;

    private NetworkCharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    [Networked] TickTimer _shootTimer { get; set; }
    
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            if (data.Move != 0 || data.Rotation != 0)
            {
                float angle = Mathf.Deg2Rad * rotationSpeed * data.Rotation * Runner.DeltaTime;
                Quaternion deltaRotation = Quaternion.AxisAngle(Vector3.up, angle);

                Vector3 moveVector = (deltaRotation * transform.forward.normalized).normalized;
                _cc.Move(5 * moveVector * data.Move * Runner.DeltaTime);
                transform.rotation = deltaRotation * transform.rotation;
            }

            if (data.Shoot && _shootTimer.Expired(Runner))
            {
                _entityManager.AddComponentData(_entity, new ShootComp());
                _shootTimer = TickTimer.CreateFromSeconds(Runner, 3);
            }
        }
    }
    
    public override void Spawned()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var playerArchetype = _entityManager.CreateArchetype(typeof(SquadData), typeof(LocalTransform), typeof(DebugSphere));
        _entity = _entityManager.CreateEntity(playerArchetype);
        _entityManager.AddComponentData(_entity, new SquadData() { MoveSpeed = 2, RotationSpeed = 4, RowCount = 4, StartUnitsCount = 100 });
        _entityManager.AddComponent<SquadSpawnTag>(_entity);
        _entityManager.AddComponent<TeamComp>(_entity);

        if (HasInputAuthority)
        {
            _entityManager.AddComponent<SquadCameraTarget>(_entity);
        }

        _entityManager.AddComponentData(_entity, new TeamComp() { PlayerId = Object.InputAuthority.PlayerId });
        _shootTimer = TickTimer.CreateFromSeconds(Runner, 3);
    }

    private void Update()
    {
        if (_entityManager.Exists(_entity))
        {
            _entityManager.SetComponentData(_entity, new LocalTransform { Position = transform.localPosition, Rotation = transform.localRotation, Scale = 1 });
        }
    }
}