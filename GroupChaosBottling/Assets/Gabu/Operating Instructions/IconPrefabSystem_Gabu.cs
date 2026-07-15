using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconPrefabSystem_Gabu : MonoBehaviour
{
    public RawImage icon;
    public TextMeshProUGUI tmpro;
    void Start()
    {
        if (icon == null || tmpro == null)
        {
            Debug.LogError("IconePrefabSystem_Gabu: Icon, Spacing or Text is null");
        }
    }
}
