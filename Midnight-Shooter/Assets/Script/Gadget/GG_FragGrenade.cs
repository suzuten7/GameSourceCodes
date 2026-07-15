using Fusion;
using System;
using UnityEngine;

/* 内容
 * ・フラググレネードの処理
 */

public class GG_FragGrenade : GG_Base
{
    [Header("◆メインオプション")]
    [SerializeField, Tooltip("最大ダメージ\n※最低値は1")]
    public float max_Damage;
    [SerializeField,Tooltip("負傷%")]float damageInjustPer = 20;
    [SerializeField, Tooltip("爆発までの時間")]
    float set_ExpTime = 5f;
    float now_ExpTime = 0f;
    [SerializeField, Tooltip("爆発サイズ\n※ダメージ判定と爆風のサイズ")]
    float set_ExpSize = 3f;
    [SerializeField, Tooltip("爆風の残る時間\n※見た目のみ")]
    float set_BlastTime = 1f;
    float now_BlastTime = 0f;

    Vector3 baseScale;

    [Header("◆サブオプション")]
    [SerializeField, Tooltip("爆風オブジェクト")] SpriteRenderer expSr;
    [SerializeField, Tooltip("ダメージ減衰曲線(距離)")] AnimationCurve lenghtDamCurve;
    [SerializeField, Tooltip("爆風の色")] Gradient blast_Gradient;
    [SerializeField, Tooltip("爆風見た目サイズ曲線")] AnimationCurve expSizeCurve;
    [SerializeField, Tooltip("壁倍率")] float wallMult = 1f;
    [SerializeField, Tooltip("ダメージスコア倍率")] float damageScoreMult = 1;
    [SerializeField, Tooltip("キルスコア倍率")] float killScoreMult = 1.1f;
    [SerializeField, Tooltip("バフ付与")] BufAdd[] bufAdds;
    [SerializeField, Tooltip("ノックバック強さ")] float kbPow;
    [SerializeField, Tooltip("ノックバック中央")] bool kbCenter;
    [Header("音")]
    [Tooltip("音最大距離")] public float soundRange;
    [Tooltip("音時間")] public float soundTime;
    [Tooltip("SEファイル")] public AudioClip seAudio;
    [Tooltip("SE音量"), Range(0, 1)] public float seVolume = 1;
    [Tooltip("SEピッチ"), Range(-3, 3)] public float sePitch = 1;
    CircleCollider2D col;
    Rigidbody2D rb;
    public bool exp;

    [Networked] bool netexp { get; set; }


    override public void Start()
    {
        exp = false;
        expSr.gameObject.SetActive(false);
        Obj_LocalObjects.Gadgets.Add(this);
        col = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        now_HP = ggdata.max_HP;
    }

    override public void Activate(bool b)
    {
        base.Activate(b);
        Library_ObjParentSet.ParentSet(gameObject, "Gadget's");
        rb.simulated = true;
        rb.AddForce(transform.rotation * Vector3.up * pm.values.now_throwPower
            * pm.values.max_throwRange * ggdata.throwSpeed);
        baseScale = expSr.transform.localScale;
    }
    override public string InfoGet()
    {
        var istr = "";
        istr += $"{LocalizSystem.LocailzSCInfo("爆破距離")}{set_ExpSize}m";
        if (max_Damage != 0)
        {
            istr += $"\n{LocalizSystem.LocailzSCInfo("基礎")}{LocalizSystem.LocailzSCInfo(max_Damage >= 0 ? "ダメージ" : "回復")}{Mathf.Abs(max_Damage)}";
            if (damageInjustPer > 0) istr += $"\n{LocalizSystem.LocailzSCInfo("負傷割合")}{damageInjustPer}%";
            istr += "\n" + Data_Base.DisInfoStr(lenghtDamCurve, set_ExpSize);
        }
        if (wallMult != 1) istr += $"\n{LocalizSystem.LocailzSCInfo("壁倍率")}{wallMult}{LocalizSystem.LocailzSCInfo("倍")}";
        if (bufAdds.Length > 0) istr += "\n" + Data_Base.BufInfoStr(bufAdds);
        if (damageScoreMult != 1) istr += $"\n{LocalizSystem.LocailzSCInfo("ダメージスコア")}{damageScoreMult}倍";
        if (killScoreMult != 1) istr += $"\n{LocalizSystem.LocailzSCInfo("キルスコア")}{killScoreMult}{LocalizSystem.LocailzSCInfo("倍")}";
        if (kbPow != 0) istr += $"\n{LocalizSystem.LocailzSCInfo("ノックバック力")}" + Mathf.Abs(kbPow);

        istr += $"\n{LocalizSystem.LocailzSCInfo("爆発音")}{soundRange}m({soundTime}{LocalizSystem.LocailzSCInfo("秒")})";
        return istr;
    }
    override protected void Update()
    {
        StopSet();
        HPServe();
        if (Net_Connect.CanControl(Object))
        {
            if (Obj_LocalObjects.TimeStopd) return;
            if (active && !exp)
            {
                now_ExpTime += Time.deltaTime;

                if (now_ExpTime >= set_ExpTime || now_HP <= 0f)
                { Explode();}
            }
        }
        else
        {
            exp = netexp;
            if (!active && netactive)
            {
                active = true;
                Library_ObjParentSet.ParentSet(gameObject, "Gadget's");
                rb.simulated = true;
                baseScale = expSr.transform.localScale;
            }
        }
        if (Obj_LocalObjects.TimeStopd) return;
        //爆風演出
        if (exp)
        {
            if (!expSr.gameObject.activeSelf)
            {
                //子供を非アクティブ化
                foreach (Transform child in transform)
                { child.gameObject.SetActive(false); }
                //爆風アクティブ化
                expSr.gameObject.SetActive(true);
            }

            now_BlastTime += Time.deltaTime;

            float t = now_BlastTime / set_BlastTime;

            //サイズ拡大
            float size = expSizeCurve.Evaluate(t) * set_ExpSize;

            expSr.transform.localScale = new Vector3(size, size, size);

            //色変化
            expSr.color = blast_Gradient.Evaluate(t);

            if (t >= 1f && Net_Connect.CanControl(Object))
            {
                Delte();
            }
        }
        Dels();
    }
    
    /// <summary>
    /// 爆破
    /// </summary>
    void Explode()
    {
        rb.bodyType = RigidbodyType2D.Static;
        if (pm != null)
        {
            float radius = set_ExpSize * 0.5f;
            Collider2D[] hits = Physics2D.OverlapCircleAll
                (transform.position, radius);
            foreach (Collider2D hit in hits)
            {
                float damage = DamegeSet(hit, max_Damage, out var exposure);
                if (exposure <= 0f) continue;
                damage *= pm.passc.damageAllAdd_Multi;
                damage *= pm.passc.damageGGAdd_Multi;
                //プレイヤーにダメージ
                if (hit.gameObject.TryGetComponent<Player_Hit>(out var ph))
                {
                    if (max_Damage != 0)
                    {
                        var damScore = damageScoreMult * pm.passc.scoreDamage_Multi;
                        var killScore = killScoreMult * pm.passc.scoreKill_Multi;
                        ph.pm.Damage(damage, damageInjustPer, false, pm, gid + (int)AtkID.Gadget, damScore, killScore);
                    }
                    if (exposure > 0)
                    {
                        ph.pm.BufChanges(bufAdds);
                        if (kbPow != 0)
                        {
                            Vector2 kbVect;
                            if (kbCenter) kbVect = transform.position - ph.pm.PosGet;
                            else kbVect = transform.up;
                            ph.pm.KBSet(kbVect.normalized * kbPow);
                        }
                    }
                }
                //プレイヤー回避
                else if(hit.gameObject.TryGetComponent<Player_Doges>(out var pdg))
                {
                    if (pdg.pm.states.teamID != pm.states.teamID) pdg.DogeAdd();
                }
                //ガジェットにダメージ
                else if (hit.gameObject.TryGetComponent<GG_Base>(out var ggb))
                { ggb.Damage(damage); }
                //壁にダメージ
                else if (hit.gameObject.TryGetComponent<Obj_Wall>(out var wall))
                { wall.Damage(damage * wallMult); }
            }
        }
        //爆風
        col.enabled = false;
        exp = true;
        RPC_ExpSet();
        Net_Value.SoundSet(transform.position, pm, soundRange, soundTime,seAudio,seVolume,sePitch);
    }

    /// <summary>
    /// グレネードダメージ
    /// </summary>
    /// <returns>±1～±100ダメージが返される (完全に隠れていたら0ダメージ)</returns>
    float DamegeSet(Collider2D hit, float max,out float exposure)
    {
        Vector2 origin = transform.position;
        Vector2 center = hit.bounds.center;
        Vector2 ext = hit.bounds.extents;

        LayerMask wallMask = LayerMask.GetMask("Wall");

        //距離減衰
        float radius = set_ExpSize * 0.5f;

        float distance = Vector2.Distance(origin, center);
        float tDist = Mathf.Clamp01(distance / radius);

        //露出率
        int sampleCount = 12;
        int visibleCount = 0;

        for (int i = 0; i < sampleCount; i++)
        {
            float angle = i * Mathf.PI * 2f / sampleCount;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            Vector2 samplePoint = center + dir * ext.magnitude;

            RaycastHit2D ray = Physics2D.Linecast(origin, samplePoint, wallMask);

            if (ray.collider == null || ray.collider == hit) visibleCount++;
        }

        exposure = (float)visibleCount / sampleCount;

        //完全に隠れていたら0ダメージ
        if (exposure <= 0f) return 0f;

        //ダメージ計算
        float sign = Mathf.Sign(max);
        float absMax = Mathf.Abs(max);

        //距離倍率
        float distDamage = Mathf.Max(1, absMax * lenghtDamCurve.Evaluate(tDist));
        //露出率を掛ける
        float damage = distDamage * exposure * sign;

        return damage;
    }

    //爆破範囲表示(Debug)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, set_ExpSize * 0.5f);
    }

    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    void RPC_ExpSet()
    {
        netexp = true;
    }
}
