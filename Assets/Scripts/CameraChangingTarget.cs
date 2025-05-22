using UnityEngine;
using Mirror;
using Cinemachine;

public class CameraChangingTarget : NetworkBehaviour
{
    public Transform cameraFollowTarget;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        CinemachineFreeLook freeLook = FindObjectOfType<CinemachineFreeLook>();

        if (freeLook != null && cameraFollowTarget != null)
        {
            freeLook.Follow = cameraFollowTarget;
            freeLook.LookAt = cameraFollowTarget;
        }
        else
        {
            Debug.LogWarning("FreeLook or target missing");
        }
    }
}