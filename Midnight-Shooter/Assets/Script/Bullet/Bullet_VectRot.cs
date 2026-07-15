using UnityEngine;

public class Bullet_VectRot : MonoBehaviour
{
    [SerializeField] Rigidbody2D rig;
    [SerializeField] float offSet;
    void Update()
    {
        var rot = transform.eulerAngles;
        rot.z = Mathf.Atan2(rig.linearVelocity.y, rig.linearVelocity.x) * Mathf.Rad2Deg + offSet;
        transform.eulerAngles = rot;
    }
}
