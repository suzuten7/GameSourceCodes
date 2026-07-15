using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
public class LocalizTest : MonoBehaviour
{
    [SerializeField] Data_Base DB;
    [SerializeField] TextMeshProUGUI tx;

    void Update()
    {
        var sdb = LocalizationSettings.StringDatabase;
        var str = "";
        for(int i = 0; i < DB.guns.Count; i++)
        {
            if(str != "")str += "\n";
            str += LocalizSystem.LocailzString("GunName", DB.guns[i].name,true);
        }
        tx.text = str;
    }
}
