using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjRots : MonoBehaviour
{
    [SerializeField] Vector3 RotSpeed;
    Vector3 Rot;
    private void Start()
    {
        Rot = transform.eulerAngles;
    }
    void FixedUpdate()
    {
        Rot += RotSpeed;
        transform.eulerAngles = Rot;
    }
}
