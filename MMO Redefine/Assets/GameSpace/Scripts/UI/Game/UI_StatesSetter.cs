namespace UIs
{
    using Datas;
    using State;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using UniVRM10;
    using static GmSystem.GS_GlobalState;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static Player.Player_Controle;
    using static UI_System;
    using static GmSystem.GS_ChangeSet;
    public class UI_StatesSetter : MonoBehaviour
    {
        [SerializeField] UI_StatusPlayer Status_MyPlayer;
        [SerializeField] Image EXPBar;
        [SerializeField] TextMeshProUGUI EXPTx;

        [SerializeField] RectTransform CharaSideRectT;
        [SerializeField] CanvasGroup STGage;
        [SerializeField] Image STBar;
        [SerializeField] Image STFill;

        [SerializeField] List<UI_StatusBase> Status_OtherPlayers;
        [SerializeField] List<UI_StatusBase> Status_Bosss;
        [SerializeField] UI_StatusBase Status_Target;
        [SerializeField] Image CrossCenterUI;
        [SerializeField] Color CrossColOff;
        [SerializeField] Color CrossColOn;
        [SerializeField] Color CrossColLook;
        [SerializeField] float LockWldMult;
        [SerializeField] RectTransform TargetUIs;
        [SerializeField] Image CrossTargetUI;
        [SerializeField] float OutMult;
        [SerializeField] TextMeshProUGUI TargetDisTx;
        [SerializeField] RectTransform TargetRot;
        [SerializeField] float RotAdd;

        [SerializeField] UI_AttackTimeSingle[] AtkTimeUIs;
        float st_FullTimer = 0f;
        float _bEXPPer = -1;
        float _bSTPer = -1;
        RaycastHit[] rayhits = new RaycastHit[50];
        void LateUpdate()
        {
            var PSta = MyPlayer;
            if (PSta == null) return;

            Status_MyPlayer.Sta = PSta;
            var sidePos = new Vector2(0.5f, 0.37f);
            sidePos.x += GetSave_Option.Cam_Pos.x * 0.004f;
            CharaSideRectT.anchorMin = sidePos;
            CharaSideRectT.anchorMax = sidePos;
            #region STアニメーション
            if (PSta.ST >= PSta.F_MST)
            {
                st_FullTimer += Time.deltaTime;
                if (st_FullTimer >= 0.5f) STGage.alpha = Mathf.Clamp01(STGage.alpha - 0.02f);
            }
            else
            {
                st_FullTimer = 0f;
                STGage.alpha = Mathf.Clamp01(STGage.alpha + 0.06f);
            }
            #endregion

            var EXPPer = Mathf.Clamp01(LPlayerVal.EXP / Mathf.Max(1f, NextEXPGet(LPlayerVal.LV)));
            if (BackPerCheck(EXPPer,_bEXPPer,1f))
            {
                _bEXPPer = EXPPer;
                EXPBar.fillAmount = EXPPer;
            }
            var EXPStr = "Lv" + LPlayerVal.LV + " Exp" + ValueStrings(LPlayerVal.EXP) + "/" + ValueStrings(NextEXPGet(LPlayerVal.LV)) + "(" + (EXPPer * 100).ToString("F1") + "%)";
            ChangeText(EXPTx,EXPStr);
            var STPer = Mathf.Clamp01(Mathf.Abs(PSta.ST) / Mathf.Max(1f, PSta.F_MST));
            if(BackPerCheck(STPer,_bSTPer,1f))
            {
                _bSTPer = STPer;
                STBar.fillAmount = STPer;
            }
            var STCol = !PSta.ChangeValues.LowST ? Color.yellow : new Color(1f, 0.5f, 0f);
            if (PSta.ST < 0) STCol = Color.blue;
            ChangeColor(STFill,STCol);

            for (int i = 0; i < Mathf.Max(Status_OtherPlayers.Count, PStaList.Count); i++)
            {
                if (Status_OtherPlayers.Count <= i) Status_OtherPlayers.Add(Instantiate(Status_OtherPlayers[0], Status_OtherPlayers[0].transform.parent));
                var opl = Status_OtherPlayers[i];
                if (i >= PStaList.Count)
                {
                    ChangeActive(opl.gameObject, false);
                    continue;
                }
                var ps = PStaList[i];
                if (ps == null || !ps.gameObject.activeSelf || ps == PSta || ps.CommonValues.Team != PSta.CommonValues.Team)
                {
                    ChangeActive(opl.gameObject, false);
                }
                else
                {
                    ChangeActive(opl.gameObject, true);
                    Status_OtherPlayers[i].Sta = PStaList[i];
                }

            }
            for (int i = 0; i < Mathf.Max(BossList.Count, Status_Bosss.Count); i++)
            {
                if (i >= Status_Bosss.Count) Status_Bosss.Add(Instantiate(Status_Bosss[0], Status_Bosss[0].transform.parent));
                var bui = Status_Bosss[i];
                if (i >= BossList.Count)
                {
                    ChangeActive(bui.gameObject, false);
                    continue;
                }
                if (BossList[i] != null && BossList[i].gameObject.activeSelf)
                {
                    var dis = Vector3.Distance(MyPlayer.PosGet, BossList[i].PosGet);
                    if (dis <= 100f)
                    {
                        ChangeActive(bui.gameObject, true);
                        bui.Sta = BossList[i];
                    }
                    else ChangeActive(bui.gameObject, false);
                }
                else ChangeActive(bui.gameObject, false);
            }

            var CrossCol = !PCont.Tr_LockOn ? CrossColOff : CrossColOn;
            if (PSta.ChangeValues.LastLookTime < 60) CrossCol = CrossColLook;
            ChangeColor(CrossCenterUI, CrossCol);
            if (PCont.Stay_LockOn && PCont.StayT_LockOn > 0.4f) PCont.Tr_LockOn = true;
            if (!PCont.Stay_LockOn && PCont.StayT_LockOn > 0 && PCont.StayT_LockOn < 0.4f) PCont.Tr_LockOn = false;
            if (PCont.Tr_LockOn && PCont.V2_Look.magnitude > 0.1f)
            {
                State_StateHit tst = null;
                var near = 200f;
                foreach (var shit in StHitList)
                {
                    if (shit == null) continue;
                    if (shit.State == null) continue;
                    var sta = shit.State;
                    if (PSta.TeamCheck(sta.CommonValues.Team) != Enum_TeamCheck.Enemy) continue;
                    if (shit.State.HP <= 0) continue;
                    var spos = PSta.PlCamera.WorldToViewportPoint(shit.PosGet);
                    if (spos.z <= 0f) continue;
                    if (spos.x < 0f || spos.x > 1f) continue;
                    if (spos.y < 0f || spos.y > 1f) continue;
                    var urang = new Vector2(spos.x - 0.5f, spos.y - 0.5f).magnitude;
                    var wrang = Vector3.Distance(PSta.PosGet, sta.PosGet) * LockWldMult;
                    urang += wrang;
                    if (near <= urang) continue;
                    var cdis = Vector3.Distance(PSta.PlCamera.transform.position, shit.PosGet);
                    var rayCount = Physics.RaycastNonAlloc(PSta.PlCamera.transform.position, shit.PosGet - PSta.PlCamera.transform.position, rayhits, cdis);
                    var NHit = false;
                    for(int i=0;i<rayCount;i++)
                    {
                        var hit = rayhits[i];
                        if (hit.collider.isTrigger) continue;
                        if (hit.collider.attachedRigidbody != null) continue;
                        if (hit.collider.TryGetComponent<State_StateBase>(out var stac)) continue;
                        if (hit.collider.TryGetComponent<State_StateHit>(out var shitc)) continue;
                        NHit = true;
                        break;
                    }
                    if (NHit) continue;
                    tst = shit;
                    near = urang;

                }
                PSta.TargetHit = tst;
            }
            if (!PCont.Tr_LockOn) PSta.TargetHit = null;

            ChangeActive(Status_Target.gameObject, PSta.TargetStaGet != null);
            Status_Target.Sta = PSta.TargetStaGet;

            if (PSta.TargetStaGet == null || !PCont.Tr_LockOn)
            {
                ChangeActive(TargetUIs.gameObject, false);
                ChangeActive(CrossTargetUI.gameObject, false);
            }
            else
            {
                ChangeActive(TargetUIs.gameObject, true);

                float canvasScale = CrossTargetUI.canvas.transform.localScale.z;
                var center = 0.5f * new Vector3(Screen.width, Screen.height);

                var VPos = PSta.PlCamera.WorldToViewportPoint(PSta.TargetPosGet);
                bool Outs = VPos.z <= 0;
                if (VPos.x < 0f || VPos.x > 1f) Outs = true;
                if (VPos.y < 0f || VPos.y > 1f) Outs = true;
                var pos = PSta.PlCamera.WorldToScreenPoint(PSta.TargetPosGet) - center;
                ChangeActive(CrossTargetUI.gameObject, VPos.z > 0);
                CrossTargetUI.rectTransform.anchoredPosition = pos / canvasScale;
                ChangeActive(TargetRot.gameObject, Outs);
                if (pos.z < 0f)
                {
                    pos.x = -pos.x;
                    pos.y = -pos.y;

                    if (Mathf.Approximately(pos.y, 0f))
                    {
                        pos.y = -center.y;
                    }
                }

                var halfSize = 0.5f * canvasScale * TargetUIs.sizeDelta;
                float d = Mathf.Max(
                    Mathf.Abs(pos.x / (center.x - halfSize.x)),
                    Mathf.Abs(pos.y / (center.y - halfSize.y))
                );
                bool isOffscreen = (pos.z < 0f || d > 1f);
                if (isOffscreen)
                {
                    pos.x /= d;
                    pos.y /= d;
                }
                pos *= OutMult;
                TargetUIs.anchoredPosition = pos / canvasScale;
                if (isOffscreen)
                {
                    TargetRot.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg + RotAdd);
                }
                var DisStr = Vector3.Distance(PSta.PosGet, PSta.TargetPosGet).ToString("F0") + "m";
                ChangeText(TargetDisTx, DisStr);
            }

            AtkTimeUISet(0,-1);
            AtkTimeUISet(1, -2);
            var atkkey = PSta.AttackVals.Keys.ToList();
            atkkey.RemoveAll(x => x < 0);
            AtkTimeUISet(2, atkkey.Count > 0 ? atkkey[atkkey.Count - 1] : 0);
        }
        void AtkTimeUISet(int Index, int AID)
        {
            var PSta = MyPlayer;
            var atui = AtkTimeUIs[Index];
            if (PSta.AttackVals.TryGetValue(AID, out var raval))
            {
                var atkEnd = 1;
                var br = raval.Attack.Branchs.Find(x => x.BID == raval.BID);
                if (br != null) atkEnd = Mathf.RoundToInt(br.EndTime * 60);
                ChangeValue(atui.TimeScroll, raval.FTime / atkEnd);
                atui.FadeUI.alpha = Mathf.Clamp01(atui.FadeUI.alpha+ 0.03f);
                var atimes = new List<Vector2Int>();
                foreach(var aev in raval.Attack.Events)
                {
                    if (aev is Data_Attack.Class_AEvent_BChange bch && (bch.IfBID == raval.BID || bch.IfBIDs.Contains(raval.BID)))
                    {
                        atimes.Add(
                            new Vector2Int
                            (
                                Mathf.RoundToInt(Mathf.Max(aev.Times.x, 0) * 60),
                                Mathf.RoundToInt(Mathf.Clamp(aev.Times.y * 60, aev.Times.x * 60, atkEnd))
                            ));
                    }
                }
                for(int i = 0; i < Mathf.Max(atimes.Count, atui.BranchScrolls.Count); i++)
                {
                    if (i >= atui.BranchScrolls.Count) atui.BranchScrolls.Add(Instantiate(atui.BranchScrolls[0], atui.BranchScrolls[0].transform.parent));
                    var bsui = atui.BranchScrolls[i];
                    if (i >= atimes.Count)
                    {
                        ChangeActive(bsui.gameObject, false);
                        continue;
                    }
                    ChangeActive(bsui.gameObject, true);
                    var leng = atimes[i].y - atimes[i].x;
                    bsui.size = Mathf.Clamp01(leng / (float)atkEnd);
                    var times = (atimes[i].x + atimes[i].y) / 2;
                    var t = times / (float)atkEnd;
                    var lt = Mathf.RoundToInt((leng + 2) * (t - 0.5f) * 2)+2;
                    //Debug.Log(i + "=" + atkEnd + ":" + leng + ":" + times + ":" + t + ":" + lt);
                    times += lt;
                    ChangeValue( bsui, Mathf.Clamp01(times / (float)atkEnd));
                    var col = Color.red;
                    switch(i % 5)
                    {
                        case 1:col = Color.yellow;break;
                        case 2:col = Color.green;break;
                        case 3:col = Color.blue;break;
                        case 4: col = Color.magenta; break;
                    }
                    col *= Mathf.Pow(0.7f, i / 5);
                    ChangeColor(bsui.handleRect.GetComponent<Image>(),col);
                }
            }
            else
            {
                ChangeValue(atui.TimeScroll, 1);
                atui.FadeUI.alpha = Mathf.Clamp01(atui.FadeUI.alpha-0.02f);
            }

        }
    }
  
}
