using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizeText : MonoBehaviour
{
    [SerializeField] private string localizationKey;
    [SerializeField] private bool useTextAsKeyIfEmpty = true;

    private TextMeshProUGUI textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (useTextAsKeyIfEmpty && string.IsNullOrEmpty(localizationKey) && textComponent != null)
        {
            localizationKey = textComponent.text;
        }
    }

    private void Start()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
            UpdateText();
        }
    }

    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }

    public void UpdateText()
    {
        if (textComponent != null && !string.IsNullOrEmpty(localizationKey))
        {
            textComponent.text = LocalizationManager.Instance.GetText(localizationKey);
        }
    }

    public void SetKey(string key)
    {
        localizationKey = key;
        UpdateText();
    }
}
