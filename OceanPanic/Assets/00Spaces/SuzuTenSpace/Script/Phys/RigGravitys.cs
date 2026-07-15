using UnityEngine;

public class RigGravitys : MonoBehaviour
{
    [SerializeField] Rigidbody Rig;
    [SerializeField] float GravPer;
    private void FixedUpdate()
    {
        Rig.linearVelocity += Physics.gravity * GravPer * 0.01f * 0.02f;
    }
}
