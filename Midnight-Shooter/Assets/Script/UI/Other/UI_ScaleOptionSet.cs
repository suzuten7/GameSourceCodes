using UnityEngine;
using UnityEngine.UI;

public class UI_ScaleOptionSet : MonoBehaviour
{
    [SerializeField] string key = "UI_Option 01";
    [SerializeField] RectTransform trans;
    private void Update()
    {
        var scale = UI_OptionManager.OptionGetFloat(key, 100) *0.01f;
        trans.localScale = Vector3.one * scale;
    }
}
