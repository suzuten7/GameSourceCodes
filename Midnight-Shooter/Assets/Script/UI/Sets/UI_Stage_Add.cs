using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Stage_Add : MonoBehaviour
{
    [SerializeField] UI_Stage_Base baseUI;
    public int ID;
    public Image backImage;
    public TextMeshProUGUI nameTx;
    public RawImage img;
    public UI_DetailSet detail;
    public void Select()
    {
        baseUI.Selectd(ID);
    }
}
