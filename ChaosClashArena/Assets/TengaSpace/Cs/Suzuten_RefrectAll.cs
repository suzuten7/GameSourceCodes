using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suzuten_RefrectAll : MonoBehaviour
{
    [SerializeField] Suzuten_ShotObj SObj;
    void FixedUpdate()
    {
        foreach(var Shots in FindObjectsOfType<Suzuten_ShotObj>())
        {
            if (Shots.UsePS != SObj.UsePS)
            {
                Shots.Refrects(SObj.UsePS, 0,3);
            }
        }
        Destroy(this);
    }
}
