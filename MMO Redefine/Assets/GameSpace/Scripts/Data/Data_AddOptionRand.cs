
namespace Datas
{
    using UnityEngine;
    using System.Linq;
    using static GmSystem.GS_GlobalState;
    using static GmSystem.GS_EnumToJpString;
    [CreateAssetMenu(menuName ="DataCre/AddOptionRand")]
    public class Data_AddOptionRand : ScriptableObject
    {
        [System.Serializable]
        public class AddOptionRand
        {
            [HideInInspector] public string DispName;
            public Enum_StateAddsType Type;
            public Enum_StateAddsOption Op;
            public float P;
            public void DispSet(int i,float Per)
            {
                DispName = "[" + i + "]" + EnumToJp(Type) + "|" + EnumToJp(Op) + "(" + P + "|" + Per.ToString("F2") + "%" + ")";
            }
        }
        public AddOptionRand[] OPs;
        [SerializeField]bool TestTo;
        [SerializeField,TextArea(10,20)]string TestTx;
        public int RandOp(out Enum_StateAddsType type, out Enum_StateAddsOption op)
        {
            int id = OPs.Length - 1;
            var p = 0f;
            for (int i = 0; i < OPs.Length; i++) p += OPs[i].P;
            var r = Random.Range(0, p);
            var s = 0f;
            for (int i = 0; i < OPs.Length; i++)
            {
                s += OPs[i].P;
                if (r <= s)
                {
                    id = i;
                    break;
                }
            }
            type = OPs[id].Type;
            op = OPs[id].Op;
            return id;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!UnityEditor.Selection.objects.Contains(this)) return;
            var p = 0f;
            for (int i = 0; i < OPs.Length; i++)
            {
                p += OPs[i].P;
            }
            for (int i = 0; i < OPs.Length; i++)
            {
                OPs[i].DispSet(i, Mathf.Max(1,OPs[i].P) / Mathf.Max(1, p) * 100);
            }
            TestTx = "";
            for (int i = 0; i < 10; i++)
            {
                var OpNum = RandOp(out var t,out var o);
                if (i > 0) TestTx += "\n";
                TestTx += "[" + i + "](" + OpNum + ")" + EnumToJp(OPs[OpNum].Type) + "|" + EnumToJp(OPs[OpNum].Op);
            }

        }
#endif
    }
}

