using UnityEngine;
using static Player_CamRot;
public class LookPCam : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null) transform.LookAt(Camera.main.transform.position);
    }
}
