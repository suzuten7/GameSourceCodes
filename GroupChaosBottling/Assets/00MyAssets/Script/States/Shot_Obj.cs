using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Statics;
using static Manifesto;
using static PlayerValue;
using static Calculation;
using static BattleManager;
public class Shot_Obj : MonoBehaviourPun
{
    public State_Base USta;
    public Rigidbody Rig;
    [SerializeField] int RemTime;
    [SerializeField] bool HitRem;
    [System.NonSerialized] public Class_Atk_Shot_Base ShotD;
    public Data_AddShot[] DelAddShots;
    public ParticleSystem[] SepParticles;
    public TrailRenderer[] SepTrails;

    public int Times = 0;
    public int SPAddCT = 0;
    public int BranchNum;
    public int AtkNum;
    bool Dels = false;
    public Dictionary<State_Base,int> HitList = new Dictionary<State_Base,int>();
    public Dictionary<State_Base, int> MultHit;
    private void Start()
    {
        Times = 0;
        SPAddCT = 0;
        ObjStrageParent(gameObject, "Shots");
    }
    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        if (USta == null) ShotDel();
        Times++;
        SPAddCT--;
        if (ShotD.HitCT > 0)
        {
            var HitKeys = HitList.Keys.ToArray();
            for (int i = 0; i < HitKeys.Length; i++)
            {
                HitList[HitKeys[i]]--;
                if (HitList[HitKeys[i]] <= 0) HitList.Remove(HitKeys[i]);
            }
        }
        if (Times >= RemTime) ShotDel();
    }
    public void ShotDel()
    {
        if (!Dels)
        {
            Dels = true;
            if (DelAddShots != null && USta != null)
                for (int i = 0; i < DelAddShots.Length; i++)
                {
                    State_Atk.ShotAdd(USta, BranchNum, DelAddShots[i], Times, transform.position, transform.eulerAngles,this,AtkNum);
                    State_Atk.SEPlayAdd(DelAddShots[i], transform.position);
                }
            photonView.RPC(nameof(RPC_SepObj), RpcTarget.All);
        }
        PhotonNetwork.Destroy(gameObject);
    }
    [PunRPC]
    void RPC_SepObj()
    {
        if (SepParticles != null)
            for (int i = 0; i < SepParticles.Length; i++)
            {
                if (SepParticles[i] == null) continue;
                ObjStrageParent(SepParticles[i].gameObject, "Effects");
                var ParMain = SepParticles[i].main;
                ParMain.loop = false;
                ParMain.stopAction = ParticleSystemStopAction.Destroy;
            }
        if (SepTrails != null)
            for (int i = 0; i < SepTrails.Length; i++)
            {
                if (SepTrails[i] == null) continue;
                ObjStrageParent(SepTrails[i].gameObject, "Effects");
                SepTrails[i].autodestruct = true;
            }

    }
    public void Hits(State_Hit HitState,Vector3 HitPos)
    {
        if (HitList.ContainsKey(HitState.Sta)) return;
        bool HitCh = false;
        for(int i=0;i< ShotD.Hits.Length; i++)
        {
            var Hit = ShotD.Hits[i];
            if (Hit.BranchNum >= 0 && BranchNum != Hit.BranchNum) continue;
            if (!TeamCheck(USta, HitState.Sta, Hit.EHit, Hit.FHit, Hit.MHit)) continue;
            if (!Hit.Heals && HitState.Sta.NoDamage) continue;
            if (!Hit.Heals && HitState.Sta.DashTime > 0)
            {
                DamageObj.DamageSet(HitPos, "Miss", Color.gray,HitState.Sta.photonView.ViewID,USta.photonView.ViewID);
                continue;
            }
            HitCh = true;
            var MultD = 1f;
            if (MultHit != null && MultHit.ContainsKey(HitState.Sta))
            {
                MultD = Mathf.Clamp(100 + Hit.MultDamChange.x * MultHit[HitState.Sta], Mathf.Min(100f, Hit.MultDamChange.y), Mathf.Max(100f, Hit.MultDamChange.y)) * 0.01f;
            }
            if (BTManager != null && BTManager.Chaos && BTManager.StageD != null && BTManager.StageD.ChaosStates != null)
            {
                var ChaosStas = BTManager.StageD.ChaosStates;
                for (int j = 0; j < ChaosStas.Length; j++)
                {
                    if (HitState.Sta.ChaosTargets(ChaosStas[j].Target) && ChaosStas[j].State == Enum_ChaosSta.弱点倍率 && MultD > 1) MultD *= 1f + (ChaosStas[j].Per * 0.01f);
                }
            }
            
            int Damage = Mathf.RoundToInt(DamSets(HitState, Hit) * MultD);
            USta.AddInfoAdd(Damage,AtkNum);
            var BreakAdd = 1f + (Hit.ShortAtk ? HitState.BreakAdds.x : HitState.BreakAdds.y)*0.01f;
            if (USta.Player)
            {
                if (BreakAdd < 0) BreakAdd *= 1f - (USta.PVal_PassiveLVGet(Enum_Passive.硬部位貫通) * 0.2f);
                if (BreakAdd > 0) BreakAdd *= 1f + (USta.PVal_PassiveLVGet(Enum_Passive.弱点破壊) * 0.3f);
                switch (Hit.DamageType)
                {
                    case Enum_DamageType.スキル:
                        if (USta.PVal_GeneSetCo(Enum_GeneTypes.一撃) >= 2) BreakAdd *= 1.7f;
                        break;
                    case Enum_DamageType.必殺:
                        if (USta.PVal_GeneSetCo(Enum_GeneTypes.一撃) >= 4) BreakAdd *= 2f;
                        break;
                }
            }
            HitState.Sta.Damage(HitPos, Damage, Hit.BreakValue * BreakAdd * MultD,false,USta.photonView.ViewID);
            if(Hit.KBs!=null)
                for(int j = 0; j < Hit.KBs.Length; j++)
                {
                    var KBd = Hit.KBs[j];
                    var KBVect = Vector3.zero;
                    var TRot = Vector3.zero;
                    switch (KBd.Base)
                    {
                        case Enum_KBBase.固定: KBVect = KBd.Pow;break;
                        case Enum_KBBase.使用者向き:KBVect = Quaternion.Euler(USta.RotGet()) * KBd.Pow; break;
                        case Enum_KBBase.弾向き:KBVect = transform.rotation * KBd.Pow; break;
                        case Enum_KBBase.対象向き:KBVect = Quaternion.Euler(HitState.Sta.RotGet()) * KBd.Pow;break;
                        case Enum_KBBase.使用者_対象方向:
                            TRot = Quaternion.LookRotation(USta.PosGet() - HitState.Sta.PosGet()).eulerAngles;
                            KBVect = Quaternion.Euler(TRot) * KBd.Pow;
                            break;
                        case Enum_KBBase.弾_対象方向:
                            TRot = Quaternion.LookRotation(transform.position - HitState.Sta.PosGet()).eulerAngles;
                            KBVect = Quaternion.Euler(TRot) * KBd.Pow;
                            break;
                    }
                    HitState.Sta.KBSet(KBVect * 0.01f, KBd.NoMass, KBd.SetVect);
                }
            if (Hit.BufSets!=null)
            for (int j = 0; j < Hit.BufSets.Length; j++) HitState.Sta.BufSets(Hit.BufSets[j],USta);
            if (SPAddCT <= 0)USta.SPAdd(Hit.SPAdd);
            if(Damage>0) USta.HitEvents(HitState.Sta,HitPos,Hit.DamageType,Hit.ShortAtk);
        }
        if (MultHit != null)
        {
            if (!MultHit.ContainsKey(HitState.Sta)) MultHit.Add(HitState.Sta, 1);
            else MultHit[HitState.Sta]++;
        }
        if (HitCh)
        {
            SPAddCT = ShotD.HitCT <= 0 ? RemTime : ShotD.HitCT;
            HitList.Add(HitState.Sta, ShotD.HitCT);
        }
        else HitList.Add(HitState.Sta, 30);
        if (HitRem && HitCh) ShotDel();
    }
    float DamSets(State_Hit HitState, Class_Atk_Shot_Hit AtkHit)
    {
        var Dam = (float)Cal(AtkHit.DamCalc, USta, HitState.Sta);
        var WeakAdd = (AtkHit.ShortAtk ? HitState.DamAdds.x : HitState.DamAdds.y);
        var DamAdd = 0f;
        if (USta.Player)
        {
            switch (AtkHit.DamageType)
            {
                case Enum_DamageType.通常:
                    DamAdd += USta.PVal_PassiveLVGet(Enum_Passive.メイン強化) * 15;
                    DamAdd += USta.PVal_PassiveLVGet(Enum_Passive.通常強化) * 20;
                    DamAdd += USta.PVal_GeneGet(Enum_GeneOptions.通常ダメージ);
                    if (USta.PVal_GeneSetCo(Enum_GeneTypes.一撃) >= 2) DamAdd -= 30f;
                    if (USta.PVal_GeneSetCo(Enum_GeneTypes.通常) >= 2) Dam += 25;
                    break;
                case Enum_DamageType.重撃:
                    DamAdd += USta.PVal_PassiveLVGet(Enum_Passive.メイン強化) * 15;
                    DamAdd += USta.PVal_PassiveLVGet(Enum_Passive.重落強化) * 25;
                    DamAdd += USta.PVal_GeneGet(Enum_GeneOptions.重撃ダメージ);
                    if (USta.PVal_GeneSetCo(Enum_GeneTypes.一撃) >= 2) DamAdd += 50f;
                    break;
                case Enum_DamageType.落下:
                    DamAdd += USta.PVal_PassiveLVGet(Enum_Passive.メイン強化) * 15;
                    DamAdd += USta.PVal_PassiveLVGet(Enum_Passive.重落強化) * 25;
                    DamAdd += USta.PVal_GeneGet(Enum_GeneOptions.落下ダメージ);
                    if (USta.PVal_GeneSetCo(Enum_GeneTypes.落下) >= 2) Dam += 30;
                    break;
                case Enum_DamageType.スキル:
                    DamAdd += USta.PVal_PassiveLVGet(Enum_Passive.スキル強化) * 25;
                    DamAdd += USta.PVal_GeneGet(Enum_GeneOptions.スキルダメージ);
                    if (USta.PVal_GeneSetCo(Enum_GeneTypes.一撃) >= 2) Dam *=1.7f;
                    if (USta.PVal_GeneSetCo(Enum_GeneTypes.スキル) >= 2) Dam += 25;
                    break;
                case Enum_DamageType.必殺:
                    DamAdd += USta.PVal_PassiveLVGet(Enum_Passive.必殺強化) * 30;
                    DamAdd += USta.PVal_GeneGet(Enum_GeneOptions.必殺ダメージ);
                    if (USta.PVal_GeneSetCo(Enum_GeneTypes.一撃) >= 4) Dam *= 2f;
                    break;
            }
            if (AtkHit.ShortAtk)
            {
                DamAdd += USta.PVal_PassiveLVGet(Enum_Passive.近距離強化) * 25;
                DamAdd += USta.PVal_GeneGet(Enum_GeneOptions.近ダメージ);
            }
            else
            {
                DamAdd += USta.PVal_PassiveLVGet(Enum_Passive.遠距離強化) * 15;
                DamAdd += USta.PVal_GeneGet(Enum_GeneOptions.遠ダメージ);
            }
            if (USta.PVal_GeneSetCo(Enum_GeneTypes.体力) >= 4 && (USta.HP / USta.FMHP) >= 0.7f) DamAdd += 30f;
            if (USta.PVal_GeneSetCo(Enum_GeneTypes.攻撃) >= 4) DamAdd += 15f;
            if (USta.PVal_GeneSetCo(Enum_GeneTypes.防御) >= 4 && USta.BufCheck(Enum_Bufs.シールド)) DamAdd += 20f;
            if (USta.PVal_GeneSetCo(Enum_GeneTypes.混沌) >= 2) DamAdd += 35f;
            if (HitState.Sta.Boss) DamAdd += USta.PVal_PassiveLVGet(Enum_Passive.ボスキラー) * 25;
            else DamAdd -= USta.PVal_PassiveLVGet(Enum_Passive.ボスキラー) * 16;
            if (WeakAdd < 0) WeakAdd *= 1f - (USta.PVal_PassiveLVGet(Enum_Passive.硬部位貫通) * 0.2f);
            if (WeakAdd > 0) WeakAdd *= 1f + (USta.PVal_PassiveLVGet(Enum_Passive.弱点破壊) * 0.3f);
            if (Random.value < USta.PVal_PassiveLVGet(Enum_Passive.クリティカルヒット) * 0.1f) Dam *= 3;
        }
        Dam *= 1f + WeakAdd * 0.01f;
        if (AtkHit.DamCalc != "" && Dam < 1) Dam = 1;

        var Regst = 1f;
        DamAdd += USta.BufPowGet(Enum_Bufs.与ダメージ増加) * 1;
        switch (AtkHit.DamageType)
        {
            case Enum_DamageType.通常:
                DamAdd += USta.BufPowGet(Enum_Bufs.メイン強化) * 1;
                DamAdd += USta.BufPowGet(Enum_Bufs.通常強化) * 1;
                break;
            case Enum_DamageType.重撃:
                DamAdd += USta.BufPowGet(Enum_Bufs.メイン強化) * 1;
                DamAdd += USta.BufPowGet(Enum_Bufs.重撃強化) * 1;
                break;
            case Enum_DamageType.落下:
                DamAdd += USta.BufPowGet(Enum_Bufs.メイン強化) * 1;
                DamAdd += USta.BufPowGet(Enum_Bufs.落下強化) * 1;
                break;
            case Enum_DamageType.スキル:
                DamAdd += USta.BufPowGet(Enum_Bufs.スキル強化) * 1;
                break; ;
            case Enum_DamageType.必殺:
                DamAdd += USta.BufPowGet(Enum_Bufs.必殺強化) * 1;
                break;
        }
        if (AtkHit.ShortAtk)
        {
            DamAdd += USta.BufPowGet(Enum_Bufs.近距離強化) * 1;
            Regst *= 1f - HitState.Sta.ShortCut * 0.01f;
        }
        else
        {
            DamAdd += USta.BufPowGet(Enum_Bufs.遠距離強化) * 1;
            Regst *= 1f - HitState.Sta.RangeCut * 0.01f;
        }
        Dam *= 1f + DamAdd * 0.01f;
        Dam *= Regst;
        if (
    (AtkHit.DamageType == Enum_DamageType.通常 ||
    AtkHit.DamageType == Enum_DamageType.重撃 ||
    AtkHit.DamageType == Enum_DamageType.落下) &&
    USta.BufCheck(Enum_Bufs.エンチャントウェポン)
    ) Dam *= 3;
        if (USta.BufCheck(Enum_Bufs.過力)) Dam *= 1.3f;
        if (AtkHit.DamCalc != "" && Dam < 1) Dam = 1;
        return Dam * (AtkHit.Heals ? -1 : 1);
    }
}
