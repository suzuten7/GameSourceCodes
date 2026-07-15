using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suzuten_RigGravity : MonoBehaviour
{
    [SerializeField] Rigidbody Rig;
    [SerializeField] float GravPower;
    void FixedUpdate()
    {
        Rig.velocity += Physics.gravity * GravPower * 0.01f;
    }
}
