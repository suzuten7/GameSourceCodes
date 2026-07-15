using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ReticleSelect_Add : MonoBehaviour
{
    [SerializeField] UI_ReticleSelect_Base baseUI;
    public int ID;
    public Image backImage;
    public TextMeshProUGUI nameTx;
    public RawImage iconIm;
    public void Select()
    {
        baseUI.Change(ID);
    }
}
