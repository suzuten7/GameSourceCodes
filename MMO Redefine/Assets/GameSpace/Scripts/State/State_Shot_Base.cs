
namespace State
{
    using Datas;
    using FNet;
    using Fusion;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using static Datas.Data_Attack;
    using static Datas.Data_Get;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalState;
    using static GmSystem.GS_GlobalValues;
    using static State.State_StateBase;
    public partial class State_Shot_Base : NetworkBehaviour
    {
        [SerializeField] bool HitWall;
        [SerializeField] bool NoPerce;
        [SerializeField] float DelTime;
        [SerializeField] Vector2 HitTime;
        [SerializeField] Data_AddShot DelAddShot;
        [SerializeField] GameObject[] SepObjs;
        [SerializeField] float SepObjLimit;
        [Header("変数")]
        public State_StateBase USta;
        public int aslot;
        [NonSerialized]public Class_AEvent_ShotMain ShotData;
        [Networked]public bool Del { get; set; }
        [Networked]public int NDummyID { get; set; }
        [Networked] public byte NEleRideID { get; set; }
        public int LDummyID;
        public byte LEleRideID;
        Dictionary<State_StateBase, int> _hcts = new ();
        public int _times = 0;
        bool hitch = false;


        private void Start()
        {
            ParentStrage(gameObject, "Shot");
            ShotList.Add(this);
        }
        void FixedUpdate()
        {
            if (LDummyID >= 0) return;
            if (Del)
            {
                foreach(var Sep in SepObjs)
                {
                    if (Sep == null) continue;
                    ParentStrage(Sep, "ShotSeps");
                    if (Sep.TryGetComponent<ParticleSystem>(out var Par)) Par.Stop();
                    Destroy(Sep,SepObjLimit);
                }
                gameObject.SetActive(false);
                return;
            }
            if (!CanControl(Object)) return;
 
            if (ShotData.HitCT > 0)
            {
                var hctKeys = _hcts.Keys.ToArray();
                foreach (var hkey in hctKeys)
                {
                    _hcts[hkey]--;
                    if (_hcts[hkey] <= 0) _hcts.Remove(hkey);
                }
            }
            _times++;
            if (_times >= DelTime * 60) Delete();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (HitWall && other.attachedRigidbody == null) Delete();
        }
        private void OnTriggerStay(Collider other)
        {
            if (LDummyID >= 0) return;
            if (!CanControl(Object)) return;
            if (HitTime != Vector2.zero)
            {
                if (Mathf.RoundToInt(HitTime.x * 60) > _times) return;
                if (Mathf.RoundToInt(HitTime.y * 60) < _times) return;
            }
            if (!other.TryGetComponent<State_StateHit>(out var shit)) return;
            if (_hcts.ContainsKey(shit.State)) return;
            _hcts.Add(shit.State, Mathf.RoundToInt(ShotData.HitCT * 60));
            Hit(other.ClosestPoint(transform.position),shit);

        }
        void Hit(Vector3 HitPos,State_StateHit sthit)
        {
            var hsta = sthit.State;
            var hit = false;
            foreach (var dhit in ShotData.Hits)
            {
                bool hitCheck = false;
                if (!dhit.Option.DeathHit && hsta.HP <= 0) continue;
                if (dhit.Option.EHit && USta.TeamCheck(hsta.CommonValues.Team) == Enum_TeamCheck.Enemy) hitCheck = true;
                if (dhit.Option.FHit && USta.TeamCheck(hsta.CommonValues.Team) == Enum_TeamCheck.Friend && hsta != USta) hitCheck = true;
                if (dhit.Option.MHit && hsta == USta) hitCheck = true;
                if (!hitCheck) continue;
                hit = true;
                var ele = !dhit.Option.EleRides ? dhit.Option.Element : (Enum_Element)LEleRideID;
                var atkval = new Struct_AtkValues
                {
                    aslot =  aslot,
                    atktype = (byte)dhit.Option.AtkType,
                    element = (byte)ele,
                    rangetype = (byte)dhit.Option.RangeType,
                };
                float hitPer = (USta.ValGet(Enum_StateAddsType.HitPer) + dhit.Option.ChangeHitPer.x) * (1f + dhit.Option.ChangeHitPer.y * 0.01f);
                if (UnityEngine.Random.value <= (100f - hitPer) * 0.01f)
                {
                    TeamGet_Str((int)hsta.CommonValues.Team, out var TColor);
                    Fusion_Reliable.TextDisp(Object.Id.Raw,USta.Object.Id.Raw,HitPos, "Miss!!!", TColor, DB.ElementColors[(int)ele]);
                    USta.AddDamage(hsta,0,atkval);
                    continue;
                }
                float dogePer = (hsta.ValGet(Enum_StateAddsType.DogePer) + dhit.Option.ChangeDogePer.x) * (1f + dhit.Option.ChangeDogePer.y * 0.01f);
                if (UnityEngine.Random.value <= (100f - dogePer) * 0.01f)
                {
                    TeamGet_Str((int)hsta.CommonValues.Team, out var TColor);
                    Fusion_Reliable.TextDisp(Object.Id.Raw, USta.Object.Id.Raw, HitPos, "Doge!!!", TColor, DB.ElementColors[(int)ele]);
                    USta.AddDamage(hsta,0, atkval);
                    continue;
                }

                var otherValue = new Dictionary<Enum_ValueBase, float>();
                if (dhit.Dam.DamCals.Length > 0)
                {
                    var defs = new Vector2(0, 1);
                    if (!dhit.Option.NoHitRegist)
                    {
                        defs.x = sthit.AllRegists.x;
                        defs.y = 1f - sthit.AllRegists.y * 0.01f;
                        switch (dhit.Option.RangeType)
                        {
                            case Enum_RangeType.Short:
                                defs.x += sthit.ShortRegists.x;
                                defs.y *= 1f - sthit.ShortRegists.y * 0.01f;
                                break;
                            case Enum_RangeType.Midle:
                                defs.x += sthit.MidleRegists.x;
                                defs.y *= 1f - sthit.MidleRegists.y * 0.01f;
                                break;
                            case Enum_RangeType.Long:
                                defs.x += sthit.LongRegists.x;
                                defs.y *= 1f - sthit.LongRegists.y * 0.01f;
                                break;
                            default:
                                defs.x += sthit.OtherRegists.x;
                                defs.y *= 1f - sthit.OtherRegists.y * 0.01f;
                                break;
                        }
                    }
                    var regHit = -defs.x * 0.01f + defs.y;

                    var dam = dhit.Dam.CalValGet(USta, hsta,new Vector2(0, defs.x), otherValue);
                    dam *= defs.y;
                    float criPer = (USta.ValGet(Enum_StateAddsType.CritPer) + dhit.Option.ChangeCritPer.x) * (1f + dhit.Option.ChangeCritPer.y * 0.01f);
                    if (UnityEngine.Random.value < criPer * 0.01f)
                    {
                        dam *= 1f + (USta.ValGet(Enum_StateAddsType.CritMult) + dhit.Option.ChangeCritMult.x) * (1f + dhit.Option.ChangeCritMult.y * 0.01f) * 0.01f;
                        atkval.crit = true;
                    }
                    if (!dhit.Option.NoDamAdd)
                    {
                        var Adds = USta.ElementAddGet(ele);
                        Adds += USta.ValGet(Enum_StateAddsType.AddDamageMult) - 100f;
                        switch (dhit.Option.AtkType)
                        {
                            case Enum_AtkType.Normal: Adds += USta.ValGet(Enum_StateAddsType.NormalDamageMult) - 100f;break;
                            case Enum_AtkType.Hev: Adds += USta.ValGet(Enum_StateAddsType.HevDamageMult) - 100f; break;
                            case Enum_AtkType.Skill: Adds += USta.ValGet(Enum_StateAddsType.SkillDamageMult) - 100f; break;
                            case Enum_AtkType.EX: Adds += USta.ValGet(Enum_StateAddsType.EXDamageMult) - 100f; break;
                        }
                        switch (dhit.Option.RangeType)
                        {
                            case Enum_RangeType.Short: Adds += USta.ValGet(Enum_StateAddsType.ShortDamageMult) - 100f; break;
                            case Enum_RangeType.Midle: Adds += USta.ValGet(Enum_StateAddsType.MidleDamageMult) - 100f; break;
                            case Enum_RangeType.Long: Adds += USta.ValGet(Enum_StateAddsType.LongDamageMult) - 100f; break;
                        }

                        dam *= Adds * 0.01f;
                    }
                    dam = Mathf.Max(1, dam);
                    if (!dhit.Option.NoBaseRegist)
                    {
                        dam *= hsta.ValGet(Enum_StateAddsType.TakeDamageRegist) * 0.01f;
                    }
                    var pvp = true;
                    if ((int)USta.CommonValues.Team > (int)Enum_Team.PLTeamA) pvp = false;
                    if ((int)USta.CommonValues.Team < (int)Enum_Team.PLTeamC) pvp = false;
                    if ((int)hsta.CommonValues.Team > (int)Enum_Team.PLTeamA) pvp = false;
                    if ((int)hsta.CommonValues.Team < (int)Enum_Team.PLTeamC) pvp = false;
                    if (pvp) dam *= NetValue.PvPDamMult * 0.01f;

                    var regEle = 1f;
                    if (!dhit.Option.NoEleRegist)
                    {
                        regEle = hsta.ElementRegistGet(ele) * 0.01f;
                        dam *= regEle;
                    }
                    if (dhit.Option.Heal)
                    {
                        dam *= USta.ValGet(Enum_StateAddsType.TakeHealMult) * 0.01f;
                        USta.AddHeal(hsta,-dam,atkval);
                    }
                    else USta.AddDamage(hsta,dam, atkval);
                    otherValue.Add(Enum_ValueBase.AddDamage, dam);
                    byte rhit = 1;
                    if (regHit > 1) rhit = 2;
                    if (regHit < 1) rhit = 0;
                    byte rele = 1;
                    if (regEle > 1) rele = 2;
                    if (regEle < 1) rele = 0;
                    hsta.Damage(USta,HitPos, dam * (!dhit.Option.Heal ? 1 : -1),atkval,rhit,rele);
                    hsta.HateAdd(USta, dam * (1f + ShotData.HateAdd.x * 0.01f),ShotData.HateAdd.y);
                }

                if (dhit.BufAdds != null)
                    foreach (var bufS in dhit.BufAdds)
                    {
                        hsta.BufSet(bufS, USta, otherValue);
                    }
            }
            if (hit && !hitch)
            {
                hitch = true;
                if (ShotData.HitEXCharge > 0) USta.EXHitdd(ShotData.HitEXCharge);
            }
            if(NoPerce && hit)Delete();    
        }
        public override void Spawned()
        {
            LDummyID = -1;
            foreach (var shot in ShotList)
            {
                if (shot == null) continue;
                if (shot.LDummyID >= 0 && shot.LDummyID == NDummyID)
                {
                    foreach (var msChild in transform.GetComponentsInChildren<Transform>())
                    {
                        Destroy(msChild.gameObject);
                    }
                    foreach (var dsChild in shot.transform.GetComponentsInChildren<Transform>())
                    {
                        var lpos = dsChild.transform.localPosition;
                        var lrot = dsChild.transform.localRotation;
                        dsChild.transform.parent = transform;
                        dsChild.transform.localPosition = lpos;
                        dsChild.transform.localRotation = lrot;
                    }
                    Destroy(shot.gameObject);
                }
            }
            LEleRideID = NEleRideID;
        }
        void Delete()
        {
            if (Runner.IsServer)
            {
                if(DelAddShot!=null)AddShot(DelAddShot,0);
                Despawn(Object);
            }
            Del = true;
        }
    }
}
