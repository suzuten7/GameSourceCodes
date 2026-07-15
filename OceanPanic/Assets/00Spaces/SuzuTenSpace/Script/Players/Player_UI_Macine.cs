using TMPro;
using UnityEngine;

public class Player_UI_Macine : MonoBehaviour
{
    [SerializeField] Player_States PSta;
    [SerializeField] GameObject MacineUIs;
    [SerializeField] TextMeshProUGUI MacineName;
    [SerializeField] TextMeshProUGUI MacineTaskPer;
    [SerializeField] bool Oni;
    void LateUpdate()
    {
        if (PSta.LastHitMacine != null && PSta.MacineHitTime > 0)
        {
            MacineUIs.SetActive(true);
            var TMacine = PSta.LastHitMacine;
            MacineName.text = "マシン-" + TMacine.MacineName;
            if (TMacine.TaskCo >= TMacine.TaskMaxs) MacineTaskPer.text = "完了済み";
            else
            {
                MacineTaskPer.text = "[" + (!Oni ? "修復中" : "破壊中") + "]";
                MacineTaskPer.text += (TMacine.TaskCo / TMacine.TaskMaxs*100f).ToString("F0") + "%";
                MacineTaskPer.text += "<size=70%>Max" + TMacine.TaskMaxs+"</size>"; 
            }
        }
        else
        {
            MacineUIs.SetActive (false);
        }
    }
}
