namespace UIs
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static UI_System;
    using State;
    using Player;
    using static GmSystem.GS_GlobalState;
    using static GmSystem.GS_ChangeSet;
    public class UI_StatusBase : MonoBehaviour
    {
        public State_StateBase Sta;
        [SerializeField] Image BackImage;
        [SerializeField]protected TextMeshProUGUI LVTx;
        [SerializeField]protected TextMeshProUGUI NameTx;
        [SerializeField] Image[] HPBars;
        [SerializeField] Image[] HPFills;
        [SerializeField] TextMeshProUGUI HPTx;
        [SerializeField] Image MPBar;
        [SerializeField] TextMeshProUGUI MPTx;
        [SerializeField] Image EXBar;
        [SerializeField] TextMeshProUGUI EXTx;
        [SerializeField] List<UI_Buf_Single> BufSUIs;
        [SerializeField] TextMeshProUGUI RevTime;
        [SerializeField] float hpr = 1;
        State_StateBase _bsta = null;
        float[] _bHpPers = new float[] { -1, -1, -1 };
        float _bMpPer=-1f;
        float _bExPer = -1f;
        virtual protected void LateUpdate()
        {
            UIUpdate();
        }
        public void UIUpdate()
        {
            if (Sta == null) return;
            if (BackImage != null)
            {
                TeamGet_Str((int)Sta.CommonValues.Team, out var TColor);
                TColor.a = BackImage.color.a;
                ChangeColor(BackImage,TColor);
            }
            NameLVSets();
            var HPPer = Mathf.Clamp01(Sta.HP / Mathf.Max(1f, Sta.F_MHP));
            var BRem = 0f;
            var poisons = Sta.BufIndexs(Enum_Buf.Poison);
            for (int i=0;i < poisons.Count; i++)
            {
                var buf = Sta.ChangeValues.Bufs[poisons[i]];
                BRem += buf.CPow * buf.CTime / 60;
            }
            var BPer = Mathf.Clamp01(BRem / Mathf.Max(1f, Sta.F_MHP));
            if(_bsta != Sta)
            {
                _bsta = Sta;
                hpr = HPPer;
            }
            var hpsdp = Time.deltaTime * 0.05f;
            if (HPPer <= 0) hpsdp *= 20;
            if (HPPer >= 1) hpsdp *= 20;
            if (hpr <= HPPer) hpr = Mathf.Min(hpr + hpsdp,HPPer);
            else hpr = Mathf.Max(hpr - hpsdp, HPPer);
            hpr = Mathf.Clamp01(hpr);

            if(hpr > HPPer)
            {
                if (BackPerCheck(hpr, _bHpPers[0], 1f))
                {
                    _bHpPers[0] = hpr;
                    ChangeFill(HPBars[0], hpr);
                }
                if (BPer > 0)
                {
                    if (BackPerCheck(HPPer, _bHpPers[1], 1f))
                    {
                        _bHpPers[1] = HPPer;
                        ChangeFill(HPBars[1], HPPer);
                    }
                }
                else ChangeFill(HPBars[1], 0);
                if (BackPerCheck(HPPer - BPer, _bHpPers[2], 1f))
                {
                    _bHpPers[2] = HPPer - BPer;
                    ChangeFill(HPBars[2], HPPer - BPer);
                }
            }
            else
            {
                if (BackPerCheck(HPPer, _bHpPers[0], 1f))
                {
                    _bHpPers[0] = HPPer;
                    ChangeFill(HPBars[0], HPPer);
                }
                if (BPer > 0)
                {
                    if (BackPerCheck(hpr, _bHpPers[1], 1f))
                    {
                        _bHpPers[1] = hpr;
                        ChangeFill(HPBars[1], hpr);
                    }
                }
                else ChangeFill(HPBars[1], 0);
                if (BackPerCheck(hpr - BPer, _bHpPers[2], 1f))
                {
                    _bHpPers[2] = hpr - BPer;
                    ChangeFill(HPBars[2], hpr - BPer);
                }
            }
            ChangeColor(HPFills[0], hpr > HPPer ? new Color(1f, 0.5f, 0f) : new Color(0.7f, 1.0f, 0.7f));
            ChangeColor(HPFills[1], Color.magenta);
            ChangeColor(HPFills[2], Color.green);

            if (HPTx != null)ChangeText(HPTx,ValueStrings(Sta.HP) + "/<size=75%>" + ValueStrings(Sta.F_MHP) + "</size>");

            if (MPBar != null)
            {
                var MPPer = Mathf.Clamp01(Sta.MP / Mathf.Max(1f, Sta.F_MMP));
                if(BackPerCheck(MPPer,_bMpPer,1f))
                {
                    _bMpPer = MPPer;
                    ChangeFill(MPBar,MPPer);
                }
                if (MPTx != null)ChangeText(MPTx,ValueStrings(Sta.MP) + "/<size=75%>" + ValueStrings(Sta.F_MMP) + "</size>");
            }
            if (EXBar != null)
            {
                var EXPer = Mathf.Clamp01(Sta.EX / Mathf.Max(1f, State_StateBase.EXMax));
                if (BackPerCheck(EXPer, _bExPer, 1f))
                {
                    _bExPer = EXPer;
                    EXBar.fillAmount = EXPer;
                }
                if (EXTx != null)ChangeText(EXTx,(Sta.EX / Mathf.Max(1f, State_StateBase.EXMax) * 100).ToString("F1") + "%");
            }
            if (BufSUIs != null)
            {
                for (int i = 0; i < Mathf.Max(Sta.ChangeValues.Bufs.Count, BufSUIs.Count); i++)
                {
                    if (BufSUIs.Count <= i) BufSUIs.Add(Instantiate(BufSUIs[0], BufSUIs[0].transform.parent));
                    var sui = BufSUIs[i];
                    if (i >= Sta.ChangeValues.Bufs.Count)
                    {
                        ChangeActive(sui.gameObject, false);
                        continue;
                    }

                    var Bufd = Sta.ChangeValues.Bufs[i];
                    ChangeActive(sui.gameObject, true);
                    sui.BufID = Bufd.ID;
                    sui.BufOp = Bufd.Op;
                    sui.BufTime.x = Bufd.CTime;
                    sui.BufTime.y = Bufd.MTime;
                    sui.BufPow.x = Bufd.CPow;
                    sui.BufPow.y = Bufd.MPow;
                }
            }
            if (RevTime != null)
            {
                var PSta = Sta.GetComponent<Player_State>();
                var Active = PSta != null && PSta.HP <= 0;
                ChangeActive(RevTime.gameObject, Active);
                if (Active)
                {
                    var RevStr = "復活まで" + Mathf.RoundToInt(PSta.RevTime - (PSta.ChangeValues.DeathTic / 60)) + "秒";
                    ChangeText(RevTime,RevStr);
                }
            }
        }
        protected virtual void NameLVSets()
        {
            if (NameTx != null)
            {
                var LVStr = Sta.CommonValues.LV.ToString();
                ChangeText(LVTx, LVStr);
                var NameStr = Sta.CommonValues.Name;
                ChangeText(NameTx, NameStr);
            }
            else if (LVTx != null)
            {
                var LVStr = "Lv" + Sta.CommonValues.LV + " " + Sta.CommonValues.Name;
                ChangeText(LVTx,LVStr);
            }
        }
    }
}
