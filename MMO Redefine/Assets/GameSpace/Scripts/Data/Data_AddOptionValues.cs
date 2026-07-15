namespace Datas
{
    using UnityEngine;
    using System.Linq;
    using static GmSystem.GS_GlobalState;
    using static GmSystem.GS_EnumToJpString;
    [CreateAssetMenu(menuName ="DataCre/AddOptionValues")]
    public class Data_AddOptionValues : ScriptableObject
    {
        [System.Serializable]
        public class Class_AddOptionValue
        {
            [HideInInspector] public string DispName;
            public Enum_StateAddsType Type;
            public Enum_StateAddsOption Op;
            public Vector2 Value;
            public void DispSet(int i)
            {
                DispName = "[" + i + "]" + EnumToJp(Type) + "|" + EnumToJp(Op) + "{" + Value.x + "+" + Value.y  + "×Lv}";
            }
        }
        public Class_AddOptionValue[] Values;
        public Class_AddOptionValue GetAddValues(Enum_StateAddsType Type, Enum_StateAddsOption Op)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (Type != Values[i].Type) continue;
                if (Op != Values[i].Op) continue;
                return Values[i];
            }
            return null;
        }
        public float GetValue(Enum_StateAddsType Type,Enum_StateAddsOption Op,int Lv)
        {
            var vals = GetAddValues(Type, Op);
            if (vals != null) return vals.Value.x + vals.Value.y * (Lv - 1);
            return 0f;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!UnityEditor.Selection.objects.Contains(this)) return;
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i].DispSet(i);
            }
        }
#endif
        
    }
}

