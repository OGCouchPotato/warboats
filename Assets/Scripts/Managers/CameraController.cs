using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    private Vector3 defenseBoardPos;
    private Vector3 defenseBoardRot;
    private Vector3 attackBoardPos;
    private Vector3 attackBoardRot;

    private Camera mainCamera;

    void Awake() {
        Instance = this;
    }

    void Start()
    {
        mainCamera = Camera.main;

        defenseBoardPos = new Vector3((float)BoardManager.Instance._width / 2 - 0.5f, 12.0f, (float)BoardManager.Instance._height / 2 - 0.5f);
        defenseBoardRot = new Vector3(90.0f, -90.0f, 0.0f);

        attackBoardPos = new Vector3(12.0f, (float)BoardManager.Instance._width / 2 - 0.5f, (float)BoardManager.Instance._height / 2 - 0.5f);
        attackBoardRot = new Vector3(0.0f, -90.0f, 0.0f);
        mainCamera.transform.SetPositionAndRotation(defenseBoardPos, Quaternion.Euler(defenseBoardRot));
    }

    public IEnumerator ToggleCameraMode(CameraMode newMode)
    {
        float time = 0;
        float duration = 1.0f;
        Vector3 selectedPos;
        Vector3 selectedRot;
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        if (newMode == CameraMode.DEFENSE)
        {
            selectedPos = defenseBoardPos;
            selectedRot = defenseBoardRot;
        }
        else
        {
            selectedPos = attackBoardPos;
            selectedRot = attackBoardRot;
        }

        while (time < duration)
        {  
            mainCamera.transform.position = Vector3.Slerp(startPos, selectedPos, time / duration);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, Quaternion.Euler(selectedRot), time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        mainCamera.transform.SetPositionAndRotation(selectedPos, Quaternion.Euler(selectedRot));
    }
}

public enum CameraMode
{
    ATTACK,
    DEFENSE
}
