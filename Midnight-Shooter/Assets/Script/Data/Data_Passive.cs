using UnityEngine;
[CreateAssetMenu(fileName = "PassiveData", menuName = "ScriptableObjects/Passive")]
public class Data_Passive : ScriptableObject
{
    [Tooltip("名前")] public string name;
    [Tooltip("説明"), TextArea] public string info;
    [Tooltip("アイコン")] public Texture icon;
    [Tooltip("アイコン色")]public Color iconColor = Color.white;
    public PassiveType type;
    public Passive passive;
    public PassiveCategory[] category;
    [Tooltip("コスト(Lv)")] public int[] costs;
    [Tooltip("変化値(Lv)")] public PassiveValue[] PassiveValues;
    [Tooltip("文字変化(Lv)")] public StringLvs[] stringLvs;
    [System.Serializable]
    public enum PassiveType
    {
        Normal,
        Trade,
        Negative,
        Cheat = 9,
    }
    [System.Serializable]
    public class PassiveValue
    {
        public PassiveValueType type;
        public float noValue = 1f;
        public float[] values;
    }
    [System.Serializable]
    public class StringLvs
    {
        public string[] strs;
    }
    public int CostGet(int lv)
    {
        int cost = 0;
        for (int k = 0; k < Mathf.Min(lv, costs.Length); k++)
        {
            cost += costs[k];
        }
        return cost;
    }
    public enum PassiveCategory
    {
        HP,
        Move,
        View,
        Shot,
        Damage,
        Gadget,
        Ult,
        Score,
        Sound,
    }
    public enum PassiveValueType
    {
        Value = -1,
        PerAdd,
        PerRem,
    }
    public string NameGet
    {
        get
        {
            var colStr = "";
            switch (type)
            {
                case PassiveType.Trade:colStr = "<color=#0000FF>";break;
                case PassiveType.Negative: colStr = "<color=#FF0000>"; break;
                case PassiveType.Cheat: colStr = "<color=#FF00FF>"; break;
            }
            return colStr + LocalizSystem.LocailzString("PassiveName", name) + (colStr == "" ? "" : "</color>"); 
        }
    }
    public string InfoGet(int clv)
    {
        var istr = LocalizSystem.LocailzString("PassiveInfo",name,false,info);
        for(int i = 0; i < PassiveValues.Length; i++)
        {
            var pstr = "(";
            for(int k = 0; k < PassiveValues[i].values.Length; k++)
            {
                if (k > 0) pstr += "|";
                if (k == clv) pstr += "<color=#BBBB00>";
                else if (k < clv) pstr += "<color=#00BB00>";
                else pstr += "<color=#BB0000>";
                var val = PassiveValues[i].values[k];
                switch (PassiveValues[i].type)
                {
                    case PassiveValueType.Value:pstr += val.ToString(); break;
                    case PassiveValueType.PerAdd:pstr += ((val - 1) * 100).ToString("F0");break;
                    case PassiveValueType.PerRem: pstr += ((1 - val) * 100).ToString("F0"); break;
                }
                pstr += "</color>";
            }
            pstr += ")";
            istr = istr.Replace("{values[" + i + "]}", pstr);
        }
        for (int i = 0; i < stringLvs.Length; i++)
        {
            var pstr = "(";
            for (int k = 0; k < stringLvs[i].strs.Length; k++)
            {
                if (k > 0) pstr += "|";
                if (k == clv) pstr += "<color=#BBBB00>";
                else if (k < clv) pstr += "<color=#00BB00>";
                else pstr += "<color=#BB0000>";
                pstr += LocalizSystem.LocailzString("PassiveInfo",name + "{str:" + i + "_" + k +"}",false, stringLvs[i].strs[k]);
                pstr += "</color>";
            }
            pstr += ")";
            istr = istr.Replace("{stringlvs[" + i + "]}", pstr);
        }
        return istr;
    }
}
