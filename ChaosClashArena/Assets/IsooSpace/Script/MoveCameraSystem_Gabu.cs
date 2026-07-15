using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveCameraSystem_Gabu : MonoBehaviour
{
    [SerializeField] private PlayerInput isg;
    public Vector2 cameraVector;
    private Vector3 nowPosition;
    private void Update()
    {
        cameraVector = isg.actions["Camera"].ReadValue<Vector2>();
        Debug.Log(cameraVector);
        nowPosition = transform.position;
        transform.position =
            new Vector3(nowPosition.x + cameraVector.x, nowPosition.y + cameraVector.y, nowPosition.z);
    }
}
