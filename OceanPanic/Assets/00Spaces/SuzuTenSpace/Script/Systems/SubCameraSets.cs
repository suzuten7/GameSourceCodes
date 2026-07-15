using UnityEngine;

public class SubCameraSets : MonoBehaviour
{
    [SerializeField] Camera BaseCamera;
    [SerializeField] Camera SubCamera;

    void LateUpdate()
    {
        SubCamera.orthographicSize = BaseCamera.orthographicSize;
        SubCamera.fieldOfView = BaseCamera.fieldOfView;
    }
}
