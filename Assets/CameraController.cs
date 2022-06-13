using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    enum CameraMode {
        PLAYERBOARD,
        OPPONENTBOARD
    }

    [Header("Player Board Camera View")]
    public Vector3 playerBoardPos;
    public Vector3 playerBoardRot;

     [Header("Opponent Board Camera View")]
    public Vector3 oppBoardPos;
    public Vector3 oppBoardRot;

    Camera mainCamera;
    CameraMode currentMode;
    CameraMode previousMode;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        currentMode = CameraMode.PLAYERBOARD;
        mainCamera.transform.SetPositionAndRotation(playerBoardPos, Quaternion.Euler(playerBoardRot));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
