using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
public class LocalizTMPText : MonoBehaviour
{
    [SerializeField] string tableStr = "UI";
    TextMeshProUGUI tx;
    string keyStr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tx = GetComponent<TextMeshProUGUI>();
        if (tx == null) enabled = false;
        keyStr = tx.text;
        Refresh();
    }
    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        Refresh();
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }
    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        Refresh();
    }
    void Refresh()
    {
        if (tx == null) return;
        tx.text = LocalizSystem.LocailzString(tableStr, keyStr);
    }
}
