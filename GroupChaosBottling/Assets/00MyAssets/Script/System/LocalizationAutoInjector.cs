using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン内のTextMeshProUGUIをスキャンし、自動的にローカライズ対象にするスクリプト。
/// LocalizationManagerと同じオブジェクトにアタッチして使用することを想定しています。
/// </summary>
public class LocalizationAutoInjector : MonoBehaviour
{
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InjectLocalizeComponents();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InjectLocalizeComponents();
    }

    /// <summary>
    /// シーン内のすべてのTextMeshProUGUIを走査し、LocalizeTextがなければ追加します。
    /// </summary>
    public void InjectLocalizeComponents()
    {
        TextMeshProUGUI[] allText = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        
        foreach (var text in allText)
        {
            // プレハブの元データなどは除外（シーン内のインスタンスのみ対象）
            if (text.gameObject.scene.name == null) continue;

            if (text.GetComponent<LocalizeText>() == null)
            {
                var localize = text.gameObject.AddComponent<LocalizeText>();
                // 初回の更新を強制
                localize.UpdateText();
            }
        }
        Debug.Log($"Localization Injected to {allText.Length} components.");
    }
}
