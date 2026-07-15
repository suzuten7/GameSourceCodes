using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suzuten_RandomVects : MonoBehaviour
{
    [SerializeField] Rigidbody Rig;
    [SerializeField] Vector2 PowRanges;

    void Start()
    {
        Vector3 Vects = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f);
        Rig.velocity = Vects * Random.Range(PowRanges.x, PowRanges.y);
    }
}
