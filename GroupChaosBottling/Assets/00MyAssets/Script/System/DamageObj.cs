using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static DataBase;
using static PlayerValue;
using static BattleManager;
using static Statics;
using Photon.Pun;

public class DamageObj : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DamText;
    [SerializeField] float HoriSpeed;
    [SerializeField] float VertSpeed;
    [SerializeField] float FallTimePer;
    float RemTime;
    Vector3 MoveVect;
    int Stack = 0;
    int times = 0;
    float Val;
    Color Col;
    int MatType = 0;

    int HitViewID;
    int AddViewID;

    static List<DamageObj> DObjs = new List<DamageObj>();

    static public void DamageSet(Vector3 Pos,int Val,Color Col,int HitViewID, int AddViewID)
    {
        DamageSet(Pos,Val.ToString(),Col,HitViewID,AddViewID);
    }
    static public void DamageSet(Vector3 Pos, string Tx, Color Col, int HitViewID, int AddViewID)
    {
        DamageObj DamObj = null;
        bool Vald = float.TryParse(Tx, out var oVal);
        var MType = 0;
        var HitView = PhotonNetwork.GetPhotonView(HitViewID);
        var HSta = HitView != null ? HitView.GetComponent<State_Base>() : null;
        var AddView = PhotonNetwork.GetPhotonView(AddViewID);
        var ASta = AddView != null ? AddView.GetComponent<State_Base>() : null;
        bool Check = false;
        if (BTManager.LocalCharas[0] != null)
        {
            if (HSta != null && HSta.Team == BTManager.LocalCharas[0].Team) Check = true;
            if (ASta != null && ASta.Team == BTManager.LocalCharas[0].Team) Check = true;
        }
        if (Check)
        {
            for (int i = 0; i < 4; i++)
            {
                if (BTManager.LocalCharas[i] == null) continue;
                var LCViewID = BTManager.LocalCharas[i].photonView.ViewID;
                if (LCViewID == HitViewID || LCViewID == AddViewID) MType = i + 1;
            }
            if (MType == 0)
            {
                if (HSta != null)
                {
                    if (HSta.PLValues != null)
                    {
                        if (HSta.PLValues.SubID <= 0) MType = 3;
                        else MType = 4;
                    }
                    else if (ASta.PLValues != null)
                    {
                        if (ASta.PLValues.SubID <= 0) MType = 3;
                        else MType = 4;
                    }
                    else if (HSta != null)
                    {
                        if (HSta.photonView.IsMine) MType = 5;
                        else MType = 6;
                    }
                    else if (ASta != null)
                    {
                        if (ASta.photonView.IsMine) MType = 5;
                        else MType = 6;
                    }

                }
            }
        }



        if (PSaves.DamStack > 0)
        {
            for (int i = DObjs.Count - 1; i >= 0; i--)
            {
                var DObjd = DObjs[i];

                if (DObjd == null || DObjd.times > PSaves.DamStack)
                {
                    DObjs.RemoveAt(i);
                    continue;
                }
                if (Col != DObjd.Col) continue;
                if (MType != DObjd.MatType) continue;
                if (!Vald && Tx != DObjd.DamText.text) continue;
                if (HitViewID != DObjd.HitViewID) continue;
                if (AddViewID != DObjd.AddViewID) continue;
                DamObj = DObjd;
            }
        }
        if (DamObj == null)
        {
            if (PSaves.DamTime <= 0) return;
            DamObj = Instantiate(DB.DamageObjs, Pos, Quaternion.identity);
            DamObj.Col = Col;
            DamObj.DamText.color = Col;
            DamObj.MoveVect = new Vector3(Random.value - 0.5f, 0f, Random.value - 0.5f);
            DamObj.HitViewID = HitViewID;
            DamObj.AddViewID = AddViewID;
            DamObj.MatType = MType;
            DamObj.RemTime = PSaves.DamTime/60f;
            if (Col == new Color(2, 2, 2))
            {
                DamObj.DamText.fontSize *= 3;
                DamObj.HoriSpeed /= 1.5f;
                DamObj.VertSpeed /= 1.5f;
                DamObj.RemTime *= 1.5f;
            }
            ObjStrageParent(DamObj.gameObject, "Damages");

            DamObj.DamText.fontMaterial = DB.DamageMats[MType];
            DObjs.Add(DamObj);
        }
        else DamObj.transform.position = Vector3.Lerp(DamObj.transform.position, Pos, 0.5f);
        DamObj.Stack++;
        if (Vald) DamObj.Val += oVal;
        if (!Vald) DamObj.DamText.text = Tx;
        else DamObj.DamText.text = DamObj.Val.ToString("F0");
        if (DamObj.Stack > 1) DamObj.DamText.text += "<size=75%>(" + DamObj.Stack + "hit)</size>";

    }
    void FixedUpdate()
    {
        times++;
        var Pos = transform.position;
        var RSpeed = Mathf.Log(PSaves.DamStack + 10f, 10f);
        Pos += MoveVect.normalized * HoriSpeed * 0.01f / RSpeed;
        var FallP = Mathf.Clamp(1f - ((times/60f) / (RemTime * FallTimePer * 0.01f)), -1f, 1f);
        Pos.y += FallP * VertSpeed * 0.01f / RSpeed;
        transform.position = Pos;
        if (times >= RemTime * 60) Destroy(gameObject);
    }
    private void LateUpdate()
    {
        var Alpha = Mathf.Clamp01(1f - ((times / 60f) - (FallTimePer * 0.01f * RemTime)) / (RemTime * (1f - FallTimePer * 0.01f)));
        var Cold = Col;
        if (Col == new Color(2, 2, 2)) Cold = Color.HSVToRGB(Mathf.Repeat(times/60f, 1f), 1f, 1f);
        DamText.color = new Color(Cold.r, Cold.g, Cold.b,Col.a * Alpha);
        transform.LookAt(Camera.main.transform);
    }
}
