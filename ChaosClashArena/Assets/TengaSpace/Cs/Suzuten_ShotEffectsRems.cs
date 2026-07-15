using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suzuten_ShotEffectsRems : MonoBehaviour
{
    void Start()
    {
        foreach(var ShotsObj in FindObjectsOfType<Suzuten_ShotObj>())
        {
            Destroy(ShotsObj.gameObject);
        }
        foreach (var PSObj in FindObjectsOfType<ParticleSystem>())
        {
            if(PSObj.transform.parent==null)Destroy(PSObj.gameObject);
        }
        foreach (var TRObj in FindObjectsOfType<TrailRenderer>())
        {
            if (TRObj.transform.parent == null) Destroy(TRObj.gameObject);
        }
        Destroy(gameObject);
    }
}
