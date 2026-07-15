namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Obj;
    public class UI_FullMap_Points : MonoBehaviour
    {
        [SerializeField] UI_FullMap_Base FMapBase;
        public RectTransform RTrans;
        public Image OutImage;
        public RawImage Icon;

        public Obj_FullMapIcon ObjMapIcon;
        public int ID;

        public void Selects()
        {
            FMapBase.PointView(ID);
        }
    }
}
