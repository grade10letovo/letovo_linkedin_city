using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class CameraController : NetworkBehaviour
{
    public GameObject cameraHolder;

    public override void OnStartAuthority()
    {
        cameraHolder.SetActive(true);
    }

    public void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            cameraHolder.transform.position = transform.position;
        }
    }
}
