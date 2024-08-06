using UnityEngine;
using Mirror;

public class PlayerControllerm : NetworkBehaviour
{
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        AttachCameraToPlayer();
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            AttachCameraToPlayer();
        }
    }

    private void AttachCameraToPlayer()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CameraFollowm cameraFollow = mainCamera.GetComponent<CameraFollowm>();
            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(transform);
            }
            else
            {
                Debug.LogError("CameraFollow component not found on main camera.");
            }
        }
        else
        {
            Debug.LogError("Main camera not found.");
        }
    }
}
