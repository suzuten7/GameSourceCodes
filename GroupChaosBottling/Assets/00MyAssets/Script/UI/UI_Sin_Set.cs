using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Sin_Set : MonoBehaviour
{
    public interface SetReturn
    {
        public void ReturnID(string Type,int ID);
    }
    public SetReturn Returns;
    public string Type;
    public int ID;
    public Button Buttons;
    public Image BackImage;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Info;
    public RawImage Icon;

    public void Click()
    {
        Returns.ReturnID(Type,ID);
    }
}
