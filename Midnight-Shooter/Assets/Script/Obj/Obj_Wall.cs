using UnityEngine;
using Fusion;
public class Obj_Wall : NetworkBehaviour
{
    public float MaxHP;
    public float hp;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Collider2D colliders;
    [SerializeField] GameObject imgObj;
    [Header("破壊音")]
    [Tooltip("音最大距離")] public float soundRange;
    [Tooltip("音時間")] public float soundTime;
    [Tooltip("SEファイル")] public AudioClip seAudio;
    [Tooltip("SE音量"), Range(0, 1)] public float seVolume = 1;
    [Tooltip("SEピッチ"), Range(-3, 3)] public float sePitch = 1;
    [Networked] public float net_hp { get; set; }
    bool setup = false;
    float bnhp = -1;
    float bhp = -1;
    bool breakd = false;
    private void Start()
    {
        Obj_LocalObjects.Walls.Add(this);
    }
    private void Update()
    {
        if (Net_Connect.CanControl(Object))
        {
            if (!setup)
            {
                setup = true;
                hp = MaxHP;
            }
            if(hp > MaxHP)hp = MaxHP;
            if (bnhp != hp)
            {
                bnhp = hp;
                net_hp = hp;
            }
            if (hp <= 0)
            {
                if (!breakd)
                {
                    breakd = true;
                    Net_Value.SoundSet(transform.position, null, soundRange, soundTime, seAudio, seVolume, sePitch);
                }
            }
            else
            {
                breakd = false;
            }
        }
        else if(Object != null)
        {
            hp = net_hp;
        }
        if(bhp != hp)
        {
            bhp = hp;
            sr.material.SetFloat("_Breaks", hp / MaxHP);
            colliders.enabled = hp > 0;
            imgObj.SetActive(hp > 0);
        }
    }
    public void Damage(float val)
    {
        RPC_Damage(val);
    }
    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    void RPC_Damage(float val)
    {
        hp -= val;
    }
    public void Resets()
    {
        hp = MaxHP;
    }
}
