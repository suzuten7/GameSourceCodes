using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Sets_Base : MonoBehaviour
{
    [SerializeField] TMP_InputField setNameIn;
    [SerializeField] List<UI_Sets_Set> setUIs;
    [SerializeField] SelUIs[] selectUIs;
    [SerializeField] TextMeshProUGUI passiveTx;
    [SerializeField] List<UI_Sets_Add> selects;
    [SerializeField] List<UI_Sets_Passive> passives;
    [SerializeField] TMP_Dropdown categoryDr;

    [SerializeField] GameObject ioUI;
    [SerializeField] TMP_InputField ioDataIn;
    int type = 0;
    int btype = -1;

    [System.Serializable]
    class SelUIs
    {
        public Image backImg;
        public TextMeshProUGUI nameTx;
        public RawImage iconIm;
        public UI_DetailSet detail;
    }
    private void Start()
    {
        ioUI.gameObject.SetActive(false);
        Player_Sets.CheckLoad();
    }
    void Update()
    {
        CategoryDrSet();
        Sets();
        Selects();
        CurrentSet();
        PassiveUI();
    }
    void Sets()
    {
        if (!setNameIn.isFocused) setNameIn.text = Player_Sets.CSetGet.name;
        for (int i = 0; i < Mathf.Max(setUIs.Count, Player_Sets.Sets.Count); i++)
        {
            if (i >= Player_Sets.Sets.Count)
            {
                setUIs[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= setUIs.Count)
            {
                setUIs.Add(Instantiate(setUIs[0], setUIs[0].transform.parent));
            }
            var sui = setUIs[i];
            sui.gameObject.SetActive(true);
            sui.UISet(i, Player_Sets.CSet);
        }
    }
    void Selects()
    {
        var data = Data_Base.DB;
        for (int i = 0; i < selectUIs.Length; i++)
        {
            selectUIs[i].backImg.color = type != i ? Color.white : Color.yellow;
        }
        int count = 0;
        switch (type)
        {
            default: count = data.guns.Count; break;
            case 1: count = data.melles.Count; break;
            case 2: count = data.gadgets.Count; break;
            case 3: count = data.ults.Count; break;
            case 4: count = data.passives.Count; break;
            case 5: count = data.charaImgs.Count; break;
            case 6:count = UI_ImageLoader_Base.ImgSets.Count+1; break;
        }
        for (int i = 0; i < Mathf.Max(selects.Count, count); i++)
        {
            if (i >= count)
            {
                selects[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= selects.Count)
            {
                selects.Add(Instantiate(selects[0], selects[0].transform.parent));
            }
            var sel = selects[i];

            var id = i;
            var idstr = id.ToString("D3");
            var namestr = "";
            var selcol = Color.white;
            var dinfostr = "";
            Texture icon = null;
            var iconcol = Color.white;
            bool hide = false;
            switch (type)
            {
                default://銃
                    var gund = data.guns[id];
                    namestr = LocalizSystem.LocailzString("GunName", gund.name);
                    icon = gund.icon;
                    iconcol = gund.iconColor;
                    selcol = Player_Sets.CSetGet.gunID != id ? Color.white : Color.yellow;
                    dinfostr = gund.InfosGet(false);
                    hide = categoryDr.value > 0 && categoryDr.value != ((int)gund.category + 1);
                    break;
                case 1://近接
                    var melled = data.melles[id];
                    namestr = LocalizSystem.LocailzString("MeleeName",melled.name);
                    icon = melled.icon;
                    iconcol = melled.iconColor;
                    selcol = Player_Sets.CSetGet.meleeID != id ? Color.white : Color.yellow;
                    dinfostr = melled.InfosGet(true);
                    hide = categoryDr.value > 0 && categoryDr.value != ((int)melled.category - (int)GunCategory.Punch + 1);
                    break;
                case 2://ガシェット
                    var ggd = data.gadgets[id];
                    namestr = LocalizSystem.LocailzString("GadgetName",ggd.name);
                    icon = ggd.icon;
                    iconcol = ggd.iconColor;
                    selcol = Player_Sets.CSetGet.gadgetID != id ? Color.white : Color.yellow;
                    dinfostr = ggd.InfosGet();
                    hide = categoryDr.value > 0 && categoryDr.value != ((int)ggd.category + 1);
                    break;
                case 3://必殺
                    var ultd = data.ults[id];
                    namestr = LocalizSystem.LocailzString("UltName",ultd.name);
                    icon = ultd.icon;
                    iconcol = ultd.iconColor;
                    selcol = Player_Sets.CSetGet.ultID != id ? Color.white : Color.yellow;
                    dinfostr = ultd.InfoGet();
                    if (categoryDr.value > 0)
                    {
                        hide = true;
                        for (int k = 0; k < ultd.category.Length; k++)
                        {
                            if (categoryDr.value == ((int)ultd.category[k] + 1)) hide = false;
                        }
                    }
                    break;
                case 4://パッシブ
                    var psd = data.passives[id];
                    id = (int)psd.passive;
                    namestr = psd.NameGet + $"\n{LocalizSystem.LocailzSCInfo("コスト")}:";
                    icon = psd.icon;
                    iconcol = psd.iconColor;
                    var psetlv = PassiveGet(id);
                    int costs = 0;
                    for (int k = 0; k < psd.costs.Length; k++)
                    {
                        costs += psd.costs[k];
                        if (k > 0) namestr += "|";
                        if (k < psetlv) namestr += "<color=#00BB00>";
                        else if (k == psetlv) namestr += "<color=#BBBB00>";
                        else namestr += "<color=#BB0000>";
                        namestr += costs.ToString() + "</color>";
                    }
                    dinfostr = psd.InfoGet(psetlv);
                    if(categoryDr.value > 0)
                    {
                        hide = true;
                        for(int k = 0; k < psd.category.Length; k++)
                        {
                            if (categoryDr.value == ((int)psd.category[k] + 1)) hide = false;
                        }
                    }
                    break;
                case 5://プレイヤー外見
                    var imgd = data.charaImgs[id];
                    selcol = Player_Sets.CSetGet.charaImgID != id ? Color.white : Color.yellow;
                    namestr = LocalizSystem.LocailzString("CharaImgName",imgd.name);
                    icon = imgd.icon;
                    iconcol = imgd.iconColor;
                    dinfostr = LocalizSystem.LocailzString("CharaImgInfo",imgd.name,false,imgd.info);
                    break;
                case 6://カスタム外見
                    selcol = Player_Sets.CSetGet.loadImgID != id ? Color.white : Color.yellow;
                    
                    if (id > 0)
                    {
                        var loadim = UI_ImageLoader_Base.ImgSets[id - 1];
                        namestr = loadim.name;
                        idstr = (id-1).ToString("D3");
                        icon = loadim.IconGet;
                        iconcol = Color.white;
                        dinfostr = "";
                    }
                    else
                    {
                        namestr = "未使用";
                        idstr = "---";
                        iconcol = Color.clear;
                        dinfostr = "";
                    }
                    break;
            }

            sel.gameObject.SetActive(!hide);
            sel.id = id;
            sel.backImage.color = selcol;
            sel.nameTx.text = idstr + ":" + namestr;
            sel.iconImage.texture = icon;
            sel.iconImage.color = iconcol;
            sel.detailSet.derailTitle = idstr + ":" + namestr;
            sel.detailSet.derailMain = dinfostr;
        }

    }
    void CategoryDrSet()
    {
        if (btype == type) return;
        btype = type;
        categoryDr.ClearOptions();
        System.Object enumArray;
        var drlist = new List<TMP_Dropdown.OptionData>();
        drlist.Add(new TMP_Dropdown.OptionData("全て"));
        switch (type)
        {
            default:
                enumArray = Enum.GetValues(typeof(GunCategory));
                for (int i=0;i< ((GunCategory[])enumArray).Length; i++)
                {
                    var en = ((GunCategory[])enumArray)[i];
                    if (en >= GunCategory.Punch) continue;
                        drlist.Add(new TMP_Dropdown.OptionData(en.ToString()));
                }
                break;
            case 1:
                enumArray = Enum.GetValues(typeof(GunCategory));
                for (int i = 0; i < ((GunCategory[])enumArray).Length; i++)
                {
                    var en = ((GunCategory[])enumArray)[i];
                    if (en < GunCategory.Punch) continue;
                    drlist.Add(new TMP_Dropdown.OptionData(en.ToString()));
                }
                break;
            case 2:
                enumArray = Enum.GetValues(typeof(Data_Gadget.GadgetCategory));
                for (int i = 0; i < ((Data_Gadget.GadgetCategory[])enumArray).Length; i++)
                {
                    var en = ((Data_Gadget.GadgetCategory[])enumArray)[i];
                    drlist.Add(new TMP_Dropdown.OptionData(en.ToString()));
                }
                break;
            case 3:
                enumArray = Enum.GetValues(typeof(Data_Ult.UltCategory));
                for (int i = 0; i < ((Data_Ult.UltCategory[])enumArray).Length; i++)
                {
                    var en = ((Data_Ult.UltCategory[])enumArray)[i];
                    drlist.Add(new TMP_Dropdown.OptionData(en.ToString()));
                }
                break;
            case 4:
                enumArray = Enum.GetValues(typeof(Data_Passive.PassiveCategory));
                for (int i = 0; i < ((Data_Passive.PassiveCategory[])enumArray).Length; i++)
                {
                    var en = ((Data_Passive.PassiveCategory[])enumArray)[i];
                    drlist.Add(new TMP_Dropdown.OptionData(en.ToString()));
                }
                break;
            case 5:
                break;
            case 6:
                break;
        }
        categoryDr.AddOptions(drlist);
    }
    void CurrentSet()
    {
        var data = Data_Base.DB;
        var cgund = data.guns[Player_Sets.CSetGet.gunID];
        var selui = selectUIs[0];
        selui.nameTx.text = Player_Sets.CSetGet.gunID.ToString("D3") + ":" + LocalizSystem.LocailzString("GunName", cgund.name);
        selui.detail.derailTitle = selui.nameTx.text;
        selui.detail.derailMain = cgund.InfosGet(false);
        selui.iconIm.texture = cgund.icon;
        selui.iconIm.color = cgund.iconColor;
        var cmeleed = data.melles[Player_Sets.CSetGet.meleeID];
        selui = selectUIs[1];
        selui.nameTx.text = Player_Sets.CSetGet.meleeID.ToString("D3") + ":" + LocalizSystem.LocailzString("MeleeName",cmeleed.name);
        selui.detail.derailTitle = selui.nameTx.text;
        selui.detail.derailMain = cmeleed.InfosGet(true);
        selui.iconIm.texture = cmeleed.icon;
        selui.iconIm.color = cmeleed.iconColor;
        var cgadgetd = data.gadgets[Player_Sets.CSetGet.gadgetID];
        selui = selectUIs[2];
        selui.nameTx.text = Player_Sets.CSetGet.gadgetID.ToString("D3") + ":" + LocalizSystem.LocailzString("GadgetName",cgadgetd.name);
        selui.detail.derailTitle = selui.nameTx.text;
        selui.detail.derailMain = cgadgetd.InfosGet();
        selui.iconIm.texture = cgadgetd.icon;
        selui.iconIm.color = cgadgetd.iconColor;
        var cultd = data.ults[Player_Sets.CSetGet.ultID];
        selui = selectUIs[3];
        selui.nameTx.text = Player_Sets.CSetGet.ultID.ToString("D3") + LocalizSystem.LocailzString("UltName",cultd.name);
        selui.detail.derailTitle = selui.nameTx.text;
        selui.detail.derailMain = cultd.InfoGet();
        selui.iconIm.texture = cultd.icon;
        selui.iconIm.color = cultd.iconColor;
        var cimgd = data.charaImgs[Player_Sets.CSetGet.charaImgID];
        selui = selectUIs[5];
        selui.nameTx.text = Player_Sets.CSetGet.charaImgID.ToString("D3") + ":" + LocalizSystem.LocailzString("CharaImgName",cimgd.name);
        selui.detail.derailTitle = selui.nameTx.text;
        selui.detail.derailMain = LocalizSystem.LocailzString("CharaImgInfo",cimgd.name,false, cimgd.info);
        selui.iconIm.texture = cimgd.icon;
        selui.iconIm.color = cimgd.iconColor;
        selui = selectUIs[6];
        if (Player_Sets.CSetGet.loadImgID > 0)
        {
            var cload = UI_ImageLoader_Base.ImgSets[Player_Sets.CSetGet.loadImgID - 1];
            selui.nameTx.text = (Player_Sets.CSetGet.loadImgID - 1).ToString("D3") + ":" + cload.name;
            selui.detail.derailTitle = selui.nameTx.text;
            selui.detail.derailMain = "";
            selui.iconIm.texture = cload.IconGet;
            selui.iconIm.color = Color.white;
        }
        else
        {
            selui.nameTx.text = $"---:{LocalizSystem.LocailzSCInfo("未使用")}";
            selui.detail.derailTitle = selui.nameTx.text;
            selui.detail.derailMain = "";
            selui.iconIm.color = Color.clear;
        }
    }
    void PassiveUI()
    {
        var data = Data_Base.DB;
        for (int i = 0; i < MathF.Max(passives.Count, Player_Sets.CSetGet.passives.Count); i++)
        {
            if (i >= Player_Sets.CSetGet.passives.Count)
            {
                passives[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= passives.Count)
            {
                passives.Add(Instantiate(passives[0], passives[0].transform.parent));
            }
            var pui = passives[i];
            var pss = Player_Sets.CSetGet.passives[i];
            var psd = data.passives.Find(x => (int)x.passive == pss.x);
            pui.gameObject.SetActive(true);
            pui.id = i;
            if (psd != null)
            {
                pui.nametx.text = psd.NameGet + "Lv" + pss.y;
                pui.detailSet.derailTitle = psd.NameGet + "Lv" + pss.y;
                pui.detailSet.derailMain = psd.InfoGet(pss.y);
                int co = 0;
                for (int k = 0; k < Mathf.Min(pss.y, psd.costs.Length); k++)
                {
                    co += psd.costs[k];
                }
                pui.costtx.text = $"{LocalizSystem.LocailzSCInfo("コスト")}:" + co;
                pui.iconIm.texture = psd.icon;
                pui.iconIm.color = psd.iconColor;
            }
            else
            {
                pui.nametx.text = "???Lv" + pss.y;
                pui.detailSet.derailTitle = "???Lv" + pss.y;
                pui.detailSet.derailMain = "";
                pui.costtx.text = $"{LocalizSystem.LocailzSCInfo("コスト")}:0";
                pui.iconIm.color = Color.clear;
            }
        }
        passiveTx.text = "(" + costGet + "/" + Data_Base.DB.CostMax + ")";
        if (costGet > Data_Base.DB.CostMax) passiveTx.text += $"<color=#FF0000>{LocalizSystem.LocailzSCInfo("コストオーバー!!!")}</color>";
        var cheats = false;
        for(int i = 0; i < Player_Sets.CSetGet.passives.Count; i++)
        {
            var pss = Player_Sets.CSetGet.passives[i];
            if (pss.x >= (int)Passive.Cheat) cheats = true;
        }
        if(cheats) passiveTx.text += $"<color=#FF00FF>{LocalizSystem.LocailzSCInfo("チート使用!!!")}</color>";
    }

    public void SetNameChange()
    {
        Player_Sets.CSetGet.name = setNameIn.text;
        Player_Sets.Save();
    }
    public void SetAdd()
    {
        Player_Sets.Sets.Add(new ());
        Player_Sets.Save();
    }
    public void SetChange(int id)
    {
        Player_Sets.CSet = id;
        Player_Sets.Save();
    }
    public void TypeChange(int types)
    {
        type = types;
    }
    public void SelectChange(int id)
    {
        switch (type)
        {
            default:Player_Sets.CSetGet.gunID = id; break;
            case 1: Player_Sets.CSetGet.meleeID = id; break;
            case 2:Player_Sets.CSetGet.gadgetID = id; break;
            case 3:Player_Sets.CSetGet.ultID = id; break;
            case 4:
                int p = -1;
                for(int i=0;i< Player_Sets.CSetGet.passives.Count; i++)
                {
                    if (Player_Sets.CSetGet.passives[i].x == id)
                    {
                        p = i;
                        break;
                    }
                }

                if (p<0)Player_Sets.CSetGet.passives.Add(new Vector2Int(id, 1));
                else
                {
                    var ps = Player_Sets.CSetGet.passives[p];
                    var psd = Data_Base.PassiveDGet((Passive)ps.x);
                    if (ps.y < psd.costs.Length)
                    {
                        ps.y++;
                        Player_Sets.CSetGet.passives[p] = ps;
                    }
                }
                break;
            case 5:Player_Sets.CSetGet.charaImgID = id;break;
            case 6:Player_Sets.CSetGet.loadImgID = id;break;
        }
        Player_Sets.Save();
    }
    public void PassiveClear()
    {
        Player_Sets.CSetGet.passives.Clear();
        Player_Sets.Save();
    }
    int PassiveGet(int id)
    {
        int psetlv = 0;
        for (int k = 0; k < Player_Sets.CSetGet.passives.Count; k++)
        {
            var ps = Player_Sets.CSetGet.passives[k];
            if (ps.x != id) continue;
            psetlv = ps.y;
            break;
        }
        return psetlv;
    }
    int costGet
    {
        get
        {
            int c = 0;
            for (int i = 0; i < Player_Sets.CSetGet.passives.Count; i++)
            {
                var pss = Player_Sets.CSetGet.passives[i];
                var psd = Data_Base.PassiveDGet((Passive)pss.x);
                if (psd != null) c += psd.CostGet(pss.y);
            }
            return c;
        }
    }

    public void Inport()
    {
        if(ioDataIn.text == "")return;
        var json = Library_Aes.DecompressFromBase64(ioDataIn.text);
        var set = JsonUtility.FromJson<Player_Sets.Set>(json);
        Player_Sets.Sets[Player_Sets.CSet] = set;
        Player_Sets.Save();
    }
    public void Export()
    {
        var set = Player_Sets.CSetGet;
        var nset = new Player_Sets.Set
        {
            name = set.name,
            gunID = set.gunID,
            meleeID = set.meleeID,
            gadgetID = set.gadgetID,
            ultID = set.ultID,
            passives = set.passives.ToList(),
            charaImgID = set.charaImgID,
            loadImgID = 0,
        };
        var json = JsonUtility.ToJson(nset);
        var b64 = Library_Aes.CompressToBase64(json);
        ioDataIn.text = b64;
    }
}
