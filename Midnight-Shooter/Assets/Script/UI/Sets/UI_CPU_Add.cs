
using TMPro;
using UnityEngine;

public class UI_CPU_Add : MonoBehaviour
{
    [SerializeField] UI_CPU_Base BaseUI;
    public int ID;
    public TextMeshProUGUI IDtx;
    public TextMeshProUGUI SetTx;
    public TMP_Dropdown TeamDr;
    public TMP_Dropdown ModeDr;
    public void Rem()
    {
        BaseUI.Rem(ID);
    }
    public void SetTeam()
    {
        BaseUI.Set(0, ID);
    }
    public void SetMode()
    {
        BaseUI.Set(1, ID);
    }
    public void SetSets()
    {
        BaseUI.Set(2,ID);
    }
}
