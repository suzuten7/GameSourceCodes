using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Ren_EffectManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particle;

    //デバッグ用
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Effect(new Vector3(1, 0.5f, 1), 0);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Effect(new Vector3(1, 0.5f, 1), 1);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Effect(new Vector3(1, 0.5f, 1), 2);
        }

    }

    public void Effect(Vector3 hitPos,int damage)
    {
        ParticleSystem effect = Instantiate(particle[damage]);
        effect.transform.position = hitPos;
        effect.Play();
    }
}
