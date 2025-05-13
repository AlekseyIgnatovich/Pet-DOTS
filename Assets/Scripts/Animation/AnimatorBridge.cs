using Unity.Entities;
using UnityEngine;

public class AnimatorBridge : MonoBehaviour
{
    public Animator Animator;

    Entity _entity;
    World _world;
    EntityManager _em;

    public void Initialize(Entity entity)
    {
        _entity = entity;
        _world = World.DefaultGameObjectInjectionWorld;
        _em = _world.EntityManager;
    }

    void Update()
    {
        if (!_em.Exists(_entity))
            return;

        if (_em.HasComponent<PlayRunAnimationTag>(_entity))
        {
            Animator.Play("Run");
            _em.RemoveComponent<PlayRunAnimationTag>(_entity);
        }
    }
}