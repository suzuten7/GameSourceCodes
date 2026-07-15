using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Title_HelpVerTxs : MonoBehaviour
{
    [SerializeField] bool verSets;
    [SerializeField] bool helpSets;
    [SerializeField] int helpID = 0;
    [SerializeField] Image[] helpSelects;

    [SerializeField] TextMeshProUGUI verTx;
    [SerializeField] TextMeshProUGUI helpTx;

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        verSets = false;
        helpSets = false;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }
    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        verSets = false;
        helpSets = false;
    }
    void Update()
    {
        if (!verSets)
        {
            verSets = true;

            var verStr = "";
            for (int i = Data_Base.DB.versions.Count - 1; i >= 0; i--)
            {
                var ver = Data_Base.DB.versions[i];
                if (verStr != "") verStr += "\n\n";
                verStr += $"<size=150%>{ver.version}({ver.date})</size>\n{LocalizSystem.LocailzString("VersionInfo", ver.version, false, ver.info)}";
            }
            verTx.text = verStr;
        }
        if (!helpSets)
        {
            for (int i = 0; i < helpSelects.Length; i++) helpSelects[i].color = i != helpID ? Color.white : Color.yellow;
            helpSets = true;
            var helpStr = "";
            switch (helpID)
            {
                default:
                    for (int i = 0; i < Data_Base.DB.helps.Count; i++)
                    {
                        var help = Data_Base.DB.helps[i];
                        if (helpStr != "") helpStr += "\n\n";
                        helpStr += $"<size=150%> - {LocalizSystem.LocailzString("HelpName", help.title)} - </size>\n{LocalizSystem.LocailzString("HelpInfo", help.title, false, help.info)}";
                    }
                    break;
                case 1:
                    for(int i = 0; i < Data_Base.DB.moveTypes.Count; i++)
                    {
                        var mtype = Data_Base.DB.moveTypes[i];
                        if (helpStr != "") helpStr += "\n";
                        helpStr += $"<color=#{ColorUtility.ToHtmlStringRGB(mtype.col)}><size=150%>{LocalizSystem.LocailzString("MoveName", mtype.name)}</size></color>\n{LocalizSystem.LocailzString("MoveInfo", mtype.name,false,mtype.info)}";
                        helpStr += $"\n{LocalizSystem.LocailzSCInfo("移動速度")}{mtype.moveSpeed}{LocalizSystem.LocailzSCInfo("倍")}";
                        helpStr += $"\n{LocalizSystem.LocailzSCInfo("歩行音")}{mtype.soundRange}{LocalizSystem.LocailzSCInfo("倍")}";
                        if (mtype.damage > 0)
                        {
                            helpStr += $"\n{LocalizSystem.LocailzSCInfo("ダメージ")}{mtype.damage}/{LocalizSystem.LocailzSCInfo("秒")}";
                            helpStr += $"({LocalizSystem.LocailzSCInfo("負傷割合")}{mtype.injuryPer}%)";
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < Data_Base.DB.bufs.Count; i++)
                    {
                        var buf = Data_Base.DB.bufs[i];
                        if (helpStr != "") helpStr += "\n";
                        var info = LocalizSystem.LocailzString("BufInfo", buf.name, false, buf.info);
                        for (int k = 0; k < buf.values.Length; k++) info = info.Replace("{values[" + k + "]}", buf.values[k].ToString());
                        helpStr += $"<color=#{ColorUtility.ToHtmlStringRGB(buf.iconColor)}><size=150%>{LocalizSystem.LocailzString("BufName", buf.name)}</size></color>\n{info}";

                    }
                    break;
            }
            helpTx.text = helpStr;
        }
    }
    public void HelpChange(int id)
    {
        helpID = id;
        helpSets = false;
    }
}
