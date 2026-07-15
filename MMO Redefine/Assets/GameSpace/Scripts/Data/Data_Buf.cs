namespace Datas
{
    using UnityEngine;
    using static GmSystem.GS_GlobalState;
    [CreateAssetMenu(menuName ="DataCre/Buf")]
    public class Data_Buf : ScriptableObject
    {
        public Enum_Buf BufID;
        public string DispName;
        public Texture BufIcon;
        public Color IconCol = Color.white;
        public Color FlameCol = Color.white;
    }
}

