using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Suzuten_Effects : MonoBehaviour
{
    [SerializeField,Tooltip("対象パーティクルシステム")]
    ParticleSystem[] ParSs;
    [SerializeField,Tooltip("対象トレール")]
    TrailRenderer[] TrailRe;
    [SerializeField,Tooltip("停止")]
    bool Stop = false;
    private void Start()
    {
        if(Stop)EffectStop(false);
    }
    private void FixedUpdate()
    {
        if (Stop)
        {
            if (ParSs != null)
            {
                for (int i = 0; i < ParSs.Length; i++)
                {
                    if (ParSs[i] != null) return;
                }
            }
            if (TrailRe != null)
            {
                for (int i = 0; i < TrailRe.Length; i++)
                {
                    if (TrailRe[i] != null) return;
                }
            }
            Destroy(gameObject);
        }
    }
    public void EffectStop(bool EmStop=true)
    {
        Stop = true;
        if (ParSs != null)
        {
            foreach (var Par in ParSs)
            {
                if (Par != null)
                {
                    MainModule ParMain = Par.main;
                    if (EmStop) Par.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    else ParMain.loop = false;
                    ParMain.stopAction = ParticleSystemStopAction.Destroy;
                }
            }
        }
        if (TrailRe != null)
        {
            foreach (var Trails in TrailRe)
            {
                if (Trails != null)
                {
                    if (EmStop) Trails.emitting = false;
                    Trails.autodestruct = true;
                }
            }
        }
    }
}