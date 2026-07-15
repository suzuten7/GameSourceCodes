using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ImageLoader_Sets : MonoBehaviour
{
    [SerializeField] UI_ImageLoader_Base loadBase;
    [SerializeField] UI_ReticleLoadImg reticleImg;
    public int ID;
    public Image backImage;
    public TextMeshProUGUI nameTx;
    public RawImage img;

    public void Select()
    {
        if(loadBase != null)loadBase.selID = ID;
        if (reticleImg != null) reticleImg.Change(ID);
    }
    public void ChangeSet(bool back)
    {
        loadBase.ChangeSet(ID, back);
    }
}
