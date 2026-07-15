using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ImageLoader_Img : MonoBehaviour
{
    [SerializeField] UI_ImageLoader_Base loadBase;
    public int ID;
    public TextMeshProUGUI idTx;
    public TMP_Dropdown typeDr;
    public RawImage img;
    public TextMeshProUGUI infoTx;
    public void LoadImg()
    {
        loadBase.LoadImage(ID);
    }
    public void Change(bool back)
    {
        loadBase.ChangeData(ID,back);
    }
    public void Rem()
    {
        loadBase.RemData(ID);
    }
    public void TypeChange()
    {
        loadBase.TypeChange(ID,typeDr.value);
    }
}
