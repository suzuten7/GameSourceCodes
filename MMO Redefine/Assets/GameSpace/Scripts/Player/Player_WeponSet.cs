
namespace Player
{
    using Datas;
    using Fusion;
    using State;
    using UnityEngine;
    using static Datas.Data_Get;
    using static FNet.Fusion_Manager;
    using static UIs.UI_System;
    using static GmSystem.GS_ChangeSet;
    public class Player_WeponSet : NetworkBehaviour
    {
        [SerializeField] bool Wep2;
        [SerializeField] Player_State PSta;
        [SerializeField] Transform HandTrans;
        [SerializeField] Transform WepTrans;
        [SerializeField] Transform OffTrans;
        [SerializeField] State_Wepon2D Wep2D;
        public int _setWeponID = -9999;
        [SerializeField] GameObject _setWeponObj = null;
        [Networked]public int AnimID { get; set; }
        void FixedUpdate()
        {


            var wepSlot = !PSta.PlayerValues.WepBack ? 0 : 2;
            if (Wep2) wepSlot++;
            var wepID = PSta.PlayerValues.SetWepons[wepSlot].GID;
            var swep = wepID;

            if (PSta.PlayerValues.WeponSkin[wepSlot] >= 0)swep = PSta.PlayerValues.WeponSkin[wepSlot];
            WepSet(swep);
            if(!CanControl(Object)) return;
            var PCon = PSta.Cont;
            if (PhotoMode) PCon = PhotoContPL == PSta ? Player_Controle.PCont : null;
            if (PCon != null)
            {
                if (!Wep2 && PCon.In_AtkFBC)
                {
                    PSta.PlayerValues.WepBack = !PSta.PlayerValues.WepBack;
                    PCon.In_AtkFBC = false;
                }
            }
            var WepD = DB.Wepons.GIDGetData(wepID);
            var aid = !Wep2 ? -1 : -2;
            if (WepD != null && PSta.HP > 0)
            {

                bool enter = false;
                bool stay = false;


                if (PCon != null)
                {

                    if (!Wep2)
                    {
                        enter = PCon.In_Atk1;
                        stay = PCon.Stay_Atk1;
                    }
                    else
                    {
                        enter = PCon.In_Atk2;
                        stay = PCon.Stay_Atk2;
                    }
                }
                bool atk = false;
                if (!WepD.Stay && enter) atk = true;
                if (WepD.Stay && stay) atk = true;
                if (PhotoMode)
                {
                    var set = PhotoSetGet(PSta);
                    var at = !Wep2 ? set.AutoLAtk : set.AutoRAtk;
                    if (at)
                    {
                        atk = true;
                        enter = FixServerTime() % 10 <= 2;
                        stay = FixServerTime() % 10 <= 4;
                    }
                }
                if (atk) PSta.AttackStart(aid, WepD.Attack, transform.position, transform.eulerAngles);
                PSta.AttackInput(aid, false, enter);
                PSta.AttackInput(aid, true, stay);
            }
            else
            {
                PSta.AttackInput(aid, false, false);
                PSta.AttackInput(aid, true, false);
            }
            PSta.AttackTrans(aid, transform.position, transform.eulerAngles);
            if (PCon != null)
            {
                if (!Wep2) PCon.In_Atk1 = false;
                if (Wep2) PCon.In_Atk2 = false;
            }
        }
        private void LateUpdate()
        {
            var _model = PSta.ModelSet.ModelGet;
            var atks = false;
            if (!Wep2 && PSta.AnimValues.LAtkID > 0) atks = true;
            if (Wep2 && PSta.AnimValues.RAtkID > 0) atks = true;
            if (atks)
            {
                Animator Anim = null;
                if (_model != null)
                {
                    if (_model.TryGetComponent<State_HumanAnim>(out var hanim)) Anim = hanim.Anim;
                    else if (_model.TryGetComponent<Animator>(out var anim)) Anim = anim;
                }
                if (Anim != null)
                {

                    if (!Wep2)
                    {
                        var LHand = Anim.GetBoneTransform(HumanBodyBones.LeftHand);
                        WepTrans.position = LHand.position;
                    }
                    else
                    {
                        var RHand = Anim.GetBoneTransform(HumanBodyBones.RightHand);
                        WepTrans.position = RHand.position;
                    }
                }
                else WepTrans.position = transform.position;
                WepTrans.rotation = HandTrans.rotation;
                Wep2D.transform.localPosition = Vector3.zero;
                Wep2D.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                WepTrans.position = OffTrans.position;
                WepTrans.rotation = OffTrans.rotation;
                Wep2D.transform.localPosition = new Vector3(0,0,0.3f);
                Wep2D.transform.localRotation = Quaternion.Euler(0, !Wep2 ? -90 : 90, !Wep2 ? -90 : 90);
            }
            
        }
        void WepSet(int wepID)
        {
            if (wepID < 0 && _setWeponObj != null) Destroy(_setWeponObj);
            if (_setWeponID != wepID)
            {
                _setWeponID = wepID;
                if (_setWeponObj != null) Destroy(_setWeponObj);
                switch (ItemGIDCategoryGet(wepID))
                {
                    case Data_Items.Enum_ItemID.Wepon:
                         var WepD = DB.Wepons.GIDGetData(wepID);
                        if (WepD != null)
                        {
                            _setWeponObj = Instantiate(WepD.WeponObject, WepTrans.position, WepTrans.rotation);
                            _setWeponObj.transform.parent = WepTrans;
                            var WepAnim = _setWeponObj.GetComponent<State_WeponAnim>();
                            if (WepAnim != null)
                            {
                                WepAnim.Sta = PSta;
                                WepAnim.WepSet = this;
                                WepAnim.Wep2 = Wep2;
                            }
                            if (Wep2)
                            {
                                var scale = _setWeponObj.transform.localScale;
                                scale.x *= -1;
                                _setWeponObj.transform.localScale = scale;
                            }
                        }
                        ChangeActive(Wep2D.gameObject, false);
                        break;
                        default:
                        ChangeActive(Wep2D.gameObject, wepID >= 0);
                        var itemD = ItemGIDDataGet(wepID);
                        if (itemD != null) Wep2D.Imgs = itemD.Icon;
                        break;
                }

            }
        }
        public void AnimIDSet(int ID)
        {
            RPC_AnimIDSet(ID);
        }
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        void RPC_AnimIDSet(int ID)
        {
            AnimID = ID;
        }
    }
    
}
