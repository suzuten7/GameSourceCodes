using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static PlayerValue;
using static DataBase;
using static Statics;
using static DataColors;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;

public class UI_StageSelect : MonoBehaviour,UI_Sin_Set.SetReturn
{
    [SerializeField] TextMeshProUGUI StageInfoTx;
    [SerializeField] List<UI_Sin_Set> StageUIs;
    [SerializeField] GameObject OfflineUI;
    [SerializeField] List<Net_JoinPlayerUI> SetUIs;
    [SerializeField] TMP_Dropdown DifeDr;
    [SerializeField] Toggle ChaosT;
    private void Update()
    {
        var StageIDV = StageID;
        var DifeModeV = DifeMode;
        var ChaosSetV = ChaosSet;
        if (!PhotonNetwork.OfflineMode)
        {
            var CRoom = PhotonNetwork.CurrentRoom;
            if (CRoom != null && !PhotonNetwork.IsMasterClient)
            {
                var CPro = CRoom.CustomProperties;
                StageIDV = CPro.TryGetValue("StageID", out var oStageID) ? (int)oStageID : 0;
                DifeModeV = CPro.TryGetValue("DifeMode", out var oDifeMode) ? (int)oDifeMode : 1;
                ChaosSetV = CPro.TryGetValue("ChaosSet", out var oChaosSet) ? (bool)oChaosSet :false;
            }
        }
        var SelStageD = DB.Stages[StageIDV];
        StageInfoTx.text = SelStageD.Name;
        StageInfoTx.text += "\n" + SelStageD.Info;
        if (!SelStageD.NoClears)
        {
            float HPMult = 1f;
            float AtkMult = 1f;
            int StarDife = DifeModeV;
            if (SelStageD.ChaosBuf != "" && ChaosT.isOn) StarDife++;
            int PCount = 0;
            switch (DifeModeV)
            {
                case 0: HPMult = 0.5f; AtkMult = 0.7f; break;
                case 2: HPMult = 1.5f; AtkMult = 1.2f; break;
                case 3: HPMult = 1.8f; AtkMult = 1.4f; break;
            }
            if (PhotonNetwork.OfflineMode)
            {
                PCount++;
                if (PSaves.AddSet1 >= 0) PCount++;
                if (PSaves.AddSet2 >= 0) PCount++;
                if (PSaves.AddSet3 >= 0) PCount++;
            }
            else
            {
                var CRoom = PhotonNetwork.CurrentRoom;
                if (CRoom != null)
                {
                    var PlayerKeys = CRoom.Players.Keys.ToArray();
                    for (int i = 0; i < PlayerKeys.Length; i++)
                    {
                        PCount++;
                        var JoinCPro = CRoom.Players[PlayerKeys[i]].CustomProperties;
                        if (JoinCPro.TryGetValue("SubUse", out var oSubUse) && (bool)oSubUse) PCount++;
                    }
                }
            }
            float HPPMult = 1f + (PCount - 1) * 0.8f;
            float DeathPMult = 1f + (PCount - 1) * 0.3f;
            StageInfoTx.text += "\n";
            if (SelStageD.ClearChaosGrain > 0)
            {
                float CGDrops = SelStageD.ClearChaosGrain;
                switch (StarDife)
                {
                    case 0: CGDrops *= 0.6f;break;
                    case 2: CGDrops *= 1.3f; break;
                    case 3: CGDrops *= 1.6f; break;
                    case 4: CGDrops *= 2.0f; break;
                }
                StageInfoTx.text += "\n<size=75%>カオスグレイン:+" + CGDrops + "</size>";
            }
            if (SelStageD.GeneDropTypes != null && SelStageD.GeneDropTypes.Length > 0)
            {
                float DropMult = 1f;
                switch (StarDife)
                {
                    case 0:DropMult = 0.5f; break;
                    case 2: DropMult = 1.3f; break;
                    case 3: DropMult = 1.6f; break;
                    case 4: DropMult = 2.0f; break;
                }
                var DropCo = new Vector2Int(Mathf.RoundToInt(SelStageD.GeneDropCount.x * DropMult),
                    Mathf.RoundToInt(SelStageD.GeneDropCount.y * DropMult));
                StageInfoTx.text += "\n因子ドロップ数:" + V2Int_Rand_Str(DropCo);
                StageInfoTx.text += "<size=75%>\n";
                for (int i = 0; i < SelStageD.GeneDropTypes.Length; i++)
                {
                    StageInfoTx.text += "<" + SelStageD.GeneDropTypes[i] + ">";
                }
                StageInfoTx.text += "</size>";
            }


            StageInfoTx.text += "\n★難易度:+" + StarDife;
            StageInfoTx.text += "\n" + Manifesto.T("LABEL_TIME_LIMIT") + ":" + (SelStageD.TimeLimSec / 60).ToString("D2") + ":" + (SelStageD.TimeLimSec % 60).ToString("D2");
            StageInfoTx.text += "\n" + Manifesto.T("LABEL_TIME_STAR") + ":" + (SelStageD.TimeStar / 60).ToString("D2") + ":" + (SelStageD.TimeStar % 60).ToString("D2");
            StageInfoTx.text += "\n★死亡数条件:" + Mathf.RoundToInt(SelStageD.DeathStar * DeathPMult);
            StageInfoTx.text += "\n<size=75%>(基本値" + SelStageD.DeathStar;
            StageInfoTx.text += "×人数" + DeathPMult.ToString("F1") + ")</size>";
            StageInfoTx.text += "\nHP補正:"+ (HPMult * HPPMult * 100).ToString("F0") + "%";
            StageInfoTx.text += "\n<size=75%>(難易度" + (HPMult * 100).ToString("F0") + "%";
            StageInfoTx.text += "×人数" + HPPMult.ToString("F1") + ")</size>";
            StageInfoTx.text += "\nAtk補正:" + (AtkMult * 100).ToString("F0") + "%";
            StageInfoTx.text += "\n<color=#FF00FF>カオスバフ</color>";
            if (SelStageD.ChaosBuf != "")
            {
                StageInfoTx.text += (!ChaosSetV ? "<color=#888888>" : "<color=#FF00FF>");
                StageInfoTx.text += "\n" + SelStageD.ChaosBuf;
                StageInfoTx.text += "</color>";
            }
            else StageInfoTx.text += "<color=#888888>なし</color>";
        }


        for (int i = 0; i < DB.Stages.Count; i++)
        {
            if (StageUIs.Count <= i) StageUIs.Add(Instantiate(StageUIs[0], StageUIs[0].transform.parent));
            var StageD = DB.Stages[i];
            var SinUI = StageUIs[i];
            SinUI.Returns = this;
            SinUI.ID = i;
            SinUI.BackImage.color = DCol.ColSelects(i == StageIDV);
            SinUI.Name.text = StageD.Name;
            if (!StageD.NoClears)
            {
                SinUI.Info.text = "ソロ   ";
                for (int j = 0; j < 3; j++)
                {
                    if (Stages.SoloStars[i] <= 3) SinUI.Info.text += j < Stages.SoloStars[i] ? "★" : "☆";
                    else SinUI.Info.text += j < (Stages.SoloStars[i] - 3) ? "<color=#FF00FF>★</color>" : "★";
                }
                SinUI.Info.text += "\nマルチ";
                for (int j = 0; j < 3; j++)
                {
                    if (Stages.MultStars[i] <= 3) SinUI.Info.text += j < Stages.MultStars[i] ? "★" : "☆";
                    else SinUI.Info.text += j < (Stages.MultStars[i] - 3) ? "<color=#FF00FF>★</color>" : "★";
                }

            }
            else SinUI.Info.text = "";
            SinUI.Icon.texture = StageD.Icon;
        }
        OfflineUI.SetActive(PhotonNetwork.OfflineMode);
        if (PhotonNetwork.OfflineMode)
        {
            for(int i = 0; i < 4; i++)
            {
                if (SetUIs.Count <= i) SetUIs.Add(Instantiate(SetUIs[0], SetUIs[0].transform.parent));
                SetUIs[i].UISet(i+1, null, false);
            }
        }
        if (DifeDr.value != DifeModeV) DifeDr.value = DifeModeV;
        if (ChaosT.isOn != ChaosSetV) ChaosT.isOn = ChaosSetV;
    }
    void UI_Sin_Set.SetReturn.ReturnID(string Type, int ID)
    {
        if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient) return;
        StageID = ID;
    }
    public void DifChange()
    {
        if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient) return;
        DifeMode = DifeDr.value;
    }
    public void ChaosChange()
    {
        if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient) return;
        ChaosSet = ChaosT.isOn;
    }
}
