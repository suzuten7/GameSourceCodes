using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Sets_Passive : MonoBehaviour
{
    [SerializeField] UI_Sets_Base baseUI;
    public int id;
    public TextMeshProUGUI nametx;
    public TextMeshProUGUI costtx;
    public UI_DetailSet detailSet;
    public RawImage iconIm;
    public void lvRem()
    {
        var pass = Player_Sets.CSetGet.passives[id];
        pass.y--;
        if (pass.y <= 0)
        {
            Player_Sets.CSetGet.passives.RemoveAt(id);
        }
        else Player_Sets.CSetGet.passives[id] = pass;
        Player_Sets.Save();
    }
    public void lvAdd()
    {
        var pass = Player_Sets.CSetGet.passives[id];
        var psd = Data_Base.PassiveDGet((Passive)pass.x);
        if (psd != null && pass.y < psd.costs.Length)
        {
            pass.y++;
            Player_Sets.CSetGet.passives[id] = pass;
        }
        Player_Sets.Save();
    }
}
