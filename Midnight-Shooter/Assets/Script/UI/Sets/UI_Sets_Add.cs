using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Sets_Add : MonoBehaviour
{
    [SerializeField] UI_Sets_Base Base;
    public int id;
    public Image backImage;
    public RawImage iconImage;
    public TextMeshProUGUI nameTx;
    public UI_DetailSet detailSet;

    public void Select()
    {
        Base.SelectChange(id);
    }
}
