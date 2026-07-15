using System.Collections.Generic;
using UnityEngine;

public class Player_Sets : MonoBehaviour
{
    static bool Check = false;
    static public string PlayerName;
    static public int CSet;
    static public List<Set> Sets = new ();
    static public Set CSetGet
    {
        get
        {
            return Sets[Mathf.Min(CSet, Sets.Count - 1)];
        }
    }
    [System.Serializable]
    public class Set
    {
        public string name;
        public int gunID;
        public int meleeID;
        public int gadgetID;
        public int ultID;
        public int charaImgID = 1;
        public int loadImgID;
        public List<Vector2Int> passives = new();
        public Set()
        {
            charaImgID = 1;
        }
    }
    static public void CheckLoad()
    {
        if (Check) return;
        Check = true;
        Load();
    }
    static public void Save()
    {
        Library_SaveFiles.SaveFile("", "Player_Name", PlayerName);
        Library_SaveFiles.SaveFile("PlayerCSets", "CSet", CSet.ToString());
        Library_SaveFiles.SaveFile("PlayerCSets", "SetCount", Sets.Count.ToString());
        for(int i = 0; i < Sets.Count; i++)
        {
            Library_SaveFiles.SaveFile("PlayerCSets", "Set_"+ i, JsonUtility.ToJson(Sets[i]));
        }
    }
    static public void Load()
    {
        PlayerName = Library_SaveFiles.LoadFileStr("", "Player_Name", (Application.isEditor ? "Tomosuzu" : "Player") + Random.Range(0, 10000).ToString("D4"));

        CSet = Library_SaveFiles.LoadFileInt("PlayerCSets", "CSet", 0);
        var scount = Library_SaveFiles.LoadFileInt("PlayerCSets", "SetCount", 0);
        for (int i = 0; i < scount; i++)
        {
            var json = Library_SaveFiles.LoadFileStr("PlayerCSets", "Set_" + i, "");
            var set = JsonUtility.FromJson<Set>(json);
            if(set != null)Sets.Add(set);
        }
        if(Sets.Count <= 0)Sets.Add(new());
        CSet = Mathf.Min(CSet, Sets.Count - 1);
    }
}
