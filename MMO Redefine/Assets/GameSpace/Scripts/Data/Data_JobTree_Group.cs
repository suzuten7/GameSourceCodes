
namespace Datas
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    [CreateAssetMenu(menuName ="DataCre/JobTreeGroup")]
    public class Data_JobTree_Group : ScriptableObject
    {
        public bool Alls;
        public List<Class_Data_JobTree> JobTrees;
        [System.Serializable]
        public class Class_Data_JobTree
        {
            [HideInInspector] public string DispName;
            public Data_JobTree CTree;
            public Vector2 Pos;
            public int[] BTreeIDs;
            public int PrereLV;
            public void DispSet(int i)
            {
                var Str = "[" + i + "]";
                Str += CTree != null ? CTree.Name : "無";
                Str += "," + Pos;
                Str += ",前提数" + BTreeIDs.Length;
                Str += ",前提LV" + PrereLV;

                if (DispName != Str) DispName = Str;
            }
        }
        [System.Serializable]
        public class Class_Data_JobTreeGroupSet
        {
            public Data_JobTree_Group JTreeGroup;
            public Vector2 OffSet;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Selection.objects.Contains(this)) return;
            for (int i = 0; i < JobTrees.Count; i++)
            {
                JobTrees[i].DispSet(i);
            }
        }
#endif
    }

}
