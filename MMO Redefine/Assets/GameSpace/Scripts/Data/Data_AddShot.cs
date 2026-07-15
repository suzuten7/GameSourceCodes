namespace Datas
{
    using UnityEngine;
    using System.Linq;
    using static Data_Attack;
    [CreateAssetMenu(menuName ="DataCre/AddShot")]
    public class Data_AddShot : ScriptableObject
    {
        public Class_AEvent_ShotMain Shot;
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!UnityEditor.Selection.objects.Contains(this)) return;
            Shot.DispSet(0);
        }
#endif
    }
}

