using System.Collections.Generic;
using UnityEngine;
using System;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    public enum Language
    {
        JP = 0,
        EN = 1
    }

    [SerializeField] private Language currentLanguage = Language.JP;
    [SerializeField] private bool autoRegisterMissingKeys = true;
    private Dictionary<string, string[]> localizationData = new Dictionary<string, string[]>();

    public event Action OnLanguageChanged;

#if UNITY_EDITOR
    private HashSet<string> missingKeysSession = new HashSet<string>();
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLocalization();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLocalization()
    {
        localizationData.Clear();
        TextAsset csvFile = Resources.Load<TextAsset>("Localization");
        if (csvFile == null)
        {
            Debug.LogWarning("Localization CSV file not found in Resources! Creating a new one may be needed.");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length <= 1) return;

        // Header: Key,JP,EN...
        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = ParseCsvRow(lines[i]);
            if (row.Length >= 2)
            {
                string key = row[0].Trim();
                if (string.IsNullOrEmpty(key)) continue;

                string[] values = new string[row.Length - 1];
                for (int j = 1; j < row.Length; j++)
                {
                    values[j - 1] = row[j].Replace("\\n", "\n").Trim();
                }
                localizationData[key] = values;
            }
        }
    }

    private string[] ParseCsvRow(string line)
    {
        List<string> fields = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '\"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }
        fields.Add(currentField);
        return fields.ToArray();
    }

    public string GetText(string key)
    {
        if (string.IsNullOrEmpty(key)) return "";
        
        if (localizationData.TryGetValue(key, out string[] values))
        {
            int index = (int)currentLanguage;
            if (index < values.Length)
            {
                return values[index];
            }
        }

#if UNITY_EDITOR
        if (autoRegisterMissingKeys)
        {
            RegisterMissingKey(key);
        }
#endif

        return key;
    }

#if UNITY_EDITOR
    private void RegisterMissingKey(string key)
    {
        if (string.IsNullOrEmpty(key) || localizationData.ContainsKey(key) || missingKeysSession.Contains(key)) return;
        
        // 数値のみ、または記号のみの場合は登録しない（誤爆防止）
        if (float.TryParse(key, out _) || key.Length <= 1 && !char.IsLetter(key[0])) return;

        missingKeysSession.Add(key);
        
        try
        {
            string path = Application.dataPath + "/00MyAssets/Script/Resources/Localization.csv";
            if (System.IO.File.Exists(path))
            {
                // 改行が含まれる場合はクォートで囲む
                string safeKey = key.Contains(",") || key.Contains("\n") ? $"\"{key}\"" : key;
                // 末尾に追記 (Key, Japanese, English)
                // とりあえず英語にはキーと同じものを入れておく
                System.IO.File.AppendAllText(path, $"\n{safeKey},{safeKey},{safeKey}");
                Debug.Log($"<color=cyan>[Localization] Auto-registered missing key: {key}</color>");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to auto-register key: {e.Message}");
        }
    }
#endif

    public void SetLanguage(Language lang)
    {
        currentLanguage = lang;
        OnLanguageChanged?.Invoke();
    }

    public Language GetCurrentLanguage() => currentLanguage;

    // ヘルパーメソッド: Enumの翻訳用
    public string GetEnumText<T>(T enumValue) where T : Enum
    {
        string key = typeof(T).Name + "_" + enumValue.ToString();
        return GetText(key);
    }
}
