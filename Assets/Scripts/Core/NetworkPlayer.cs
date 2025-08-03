using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    const float rotationSpeed = 30f;
    const float moveSpeed = 1f;

    private NetworkCharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            if (data.Move == 0 && data.Rotation == 0)
                return;

            float angle = Mathf.Deg2Rad * rotationSpeed * data.Rotation * Runner.DeltaTime;
            Quaternion deltaRotation = Quaternion.AxisAngle(Vector3.up, angle);

            Vector3 moveVector = (deltaRotation * transform.forward.normalized).normalized;
            _cc.Move(5 * moveVector * data.Move * Runner.DeltaTime);
            transform.rotation = deltaRotation * transform.rotation;
        }
    }
}