using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MoblieSets : MonoBehaviour
{
    [SerializeField] UI_MobliePos[] mposs;
    [SerializeField] GameObject selectsUI;
    [SerializeField] TextMeshProUGUI selNameTx;
    [SerializeField] TextMeshProUGUI selPosTx;
    [SerializeField] TMP_InputField selScaleIn;
    [SerializeField] Slider selScaleSlider;
    public UI_MobliePos selectPosUI;
    private void Update()
    {
        selectsUI.SetActive(selectPosUI != null);
        if (selectPosUI != null)
        {
            selNameTx.text = selectPosUI.name;
            var pos = selectPosUI.rect.anchoredPosition;
            selPosTx.text = pos.x.ToString("F0")+ ","+ pos.y.ToString("F0");
            if(!selScaleIn.isFocused) selScaleIn.text = selectPosUI.scale.ToString("F0");
            selScaleSlider.value = selectPosUI.scale;
        }
    }
    public void ScaleSet()
    {
        if (selectPosUI != null)
        {
            selectPosUI.ScaleSet(selScaleSlider.value);
        }
    }
    public void ScaleSetIn()
    {
        if (selectPosUI != null && float.TryParse(selScaleIn.text, out var v))
        {
            selectPosUI.ScaleSet(v);
        }
    }
    public void SelReset()
    {
        if (selectPosUI != null)
        {
            selectPosUI.Resets();
        }
    }
    public void Resets()
    {
        for (int i = 0; i < mposs.Length; i++)
        {
            mposs[i].Resets();
        }
    }
    public void Saves()
    {
        for (int i = 0; i < mposs.Length; i++)
        {
            mposs[i].Save();
        }
    }
    public void CloseLoads()
    {
        var fmpos = FindObjectsByType<UI_MobliePos>();
        for (int i = 0; i < fmpos.Length; i++)
        {
            fmpos[i].Load();
        }
    }
}
