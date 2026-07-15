using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UI_Statistics : MonoBehaviour
{
    public enum StatEnum
    {
        Test,
        TitleTime,
        OfflineTime,
        OnlineTime,

        KillTotal_PL,
        KillConsMax_PL,
        DeathTotal_PL,
        FFTotal_PL,
        SelfDeathTotal_PL,

        ScoreTotal_PL,
        ScoreMax_PL,

        AddDamageTotal_PL,
        TakeDamageTotal_PL,
        AddHealTotal_PL,
        TakeHealTotal_PL,

        GunAtkCount_PL,
        MeleeAtkCount_PL,
        GadgetAtkCount_PL,
        UltAtkCount_PL,

        KillTotal_CPU,
        KillConsMax_CPU,
        DeathTotal_CPU,
        FFTotal_CPU,
        SelfDeathTotal_CPU,

        ScoreTotal_CPU,
        ScoreMax_CPU,

        AddDamageTotal_CPU,
        TakeDamageTotal_CPU,
        AddHealTotal_CPU,
        TakeHealTotal_CPU,

        GunAtkCount_CPU,
        MeleeAtkCount_CPU,
        GadgetAtkCount_CPU,
        UltAtkCount_CPU,
    }

    static public bool loads = false;
    static public Dictionary<StatEnum, float> Values = new();
    static public void Save()
    {
        var keys = Values.Keys.ToArray();
        for(int i = 0; i < keys.Length; i++)
        {
            Library_SaveFiles.SaveFile("Statistics", keys[i].ToString(), Values[keys[i]].ToString());
        }
    }
    static public void LoadCheck()
    {
        if (loads) return;
        loads = true;
        Values.Clear();
        var types = System.Enum.GetValues(typeof(StatEnum));
        for (int i = 0; i < types.Length; i++)
        {
            var key = (StatEnum)types.GetValue(i);
            var val = Library_SaveFiles.LoadFileFloat("Statistics", key.ToString());
            Values.TryAdd(key, val);
        }
    }
    static public void SetValue(StatEnum key,float value)
    {
        LoadCheck();
        if (!Values.ContainsKey(key)) Values.Add(key, 0);
        Values[key] = value;
    }
    static public void AddValue(StatEnum key,float value)
    {
        LoadCheck();
        if (!Values.ContainsKey(key))Values.Add(key,0);
        Values[key] += value;
    }
    static public float GetValue(StatEnum key)
    {
        if (!Values.ContainsKey(key)) return 0;
        return Values[key];
    }
    [SerializeField] TextMeshProUGUI tx;

    private void Start()
    {
        LoadCheck();
    }
    void Update()
    {
        var str = "";
        var types = System.Enum.GetValues(typeof(StatEnum));

        for (int i = 0; i < types.Length; i++)
        {
            var type = (StatEnum)types.GetValue(i);
            if (i != 0) str += "\n";
            str += $"{LocalizSystem.LocailzString("Statistics", type.ToString())}:{GetValue(type)}";
        }
        tx.text = str;
    }
}
