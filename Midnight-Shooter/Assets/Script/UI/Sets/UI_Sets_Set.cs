using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Sets_Set : MonoBehaviour
{
    [SerializeField] UI_Sets_Base setBase;
    [SerializeField] UI_CPU_Base cpuBase;
    [SerializeField] int id;
    [SerializeField] Image backImage;
    [SerializeField] TextMeshProUGUI nameTx;
    [SerializeField] UI_DetailSet detail;

    public void Select()
    {
        if (setBase != null) setBase.SetChange(id);
        if(cpuBase != null)cpuBase.SetChange(id);
    }
    public void Swap(bool up)
    {
        int cid = id + (!up ? 1 : -1);
        if (cid < 0 || cid >= Player_Sets.Sets.Count) return;
        var cset = Player_Sets.Sets[cid];
        Player_Sets.Sets[cid] = Player_Sets.Sets[id];
        Player_Sets.Sets[id] = cset;
        Player_Sets.Save();
    }
    public void UISet(int _id,int select)
    {
        id = _id;
        backImage.color = _id != select ? Color.white : Color.yellow;
        var namestr = $"{LocalizSystem.LocailzSCInfo("セット")}" + (id + 1) + ":" + Player_Sets.Sets[id].name;
        nameTx.text = namestr;
        detail.derailTitle = namestr;
        var set = Player_Sets.Sets[id];
        var dtx = "";
        dtx += $"[{LocalizSystem.LocailzSCInfo("通常外見")}]" + set.charaImgID.ToString("D3") + ":" + LocalizSystem.LocailzString("CharaImgName", Data_Base.DB.charaImgs[set.charaImgID].name);
        if (set.loadImgID > 0) dtx += $"\n[{LocalizSystem.LocailzSCInfo("カスタム外見")}]" + (set.loadImgID - 1).ToString("D3") + ":" + UI_ImageLoader_Base.ImgSets[set.loadImgID - 1].name;
        else dtx += $"\n[{LocalizSystem.LocailzSCInfo("カスタム外見")}]:{LocalizSystem.LocailzSCInfo("未使用")}";
        dtx += $"\n[{LocalizSystem.LocailzSCInfo("銃")}]" + set.gunID.ToString("D3") + ":" + LocalizSystem.LocailzString("GunName",Data_Base.DB.guns[set.gunID].name);
        dtx += $"\n[{LocalizSystem.LocailzSCInfo("近接")}]" + set.meleeID.ToString("D3") + ":" + LocalizSystem.LocailzString("MeleeName",Data_Base.DB.melles[set.meleeID].name);
        dtx += $"\n[{LocalizSystem.LocailzSCInfo("ガシェット")}]" + set.gadgetID.ToString("D3") + ":" + LocalizSystem.LocailzString("GadgetName", Data_Base.DB.gadgets[set.gadgetID].name);
        dtx += $"\n[{LocalizSystem.LocailzSCInfo("必殺")}]" + set.ultID.ToString("D3") + ":" + LocalizSystem.LocailzString("UltName", Data_Base.DB.ults[set.ultID].name);
        dtx += $"\n[{LocalizSystem.LocailzSCInfo("パッシブ")}]<size=70%>";
        if (set.passives.Count <= 0) dtx += $"\n{LocalizSystem.LocailzSCInfo("無し")}";
        for(int i = 0; i < set.passives.Count; i++)
        {
            var passd = Data_Base.PassiveDGet((Passive)set.passives[i].x);
            dtx += "\n" + (passd != null ? passd.NameGet : "???") + "Lv" + set.passives[i].y;
        }
        dtx += "</size>";
        detail.derailMain = dtx;
    }
}
