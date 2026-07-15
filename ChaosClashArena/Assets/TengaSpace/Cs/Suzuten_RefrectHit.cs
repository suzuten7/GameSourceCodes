using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suzuten_RefrectHit : MonoBehaviour
{
    [SerializeField] Suzuten_ShotObj SObj;
    private void OnTriggerEnter(Collider other)
    {
        Suzuten_ShotObj Shots = other.GetComponent<Suzuten_ShotObj>();
        if (Shots != null)
        {
            if (Shots.UsePS != SObj.UsePS)
            {
                Shots.Refrects(SObj.UsePS, 1,1);
            }
        }
    }
}
