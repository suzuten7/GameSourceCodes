namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static Datas.Data_Get;
    using static GmSystem.GS_GlobalState;
    using static UI_System;
    using static GmSystem.GS_EnumToJpString;
    using static GmSystem.GS_ChangeSet;
    public class UI_Buf_Single : MonoBehaviour
    {
        public int BufID;
        public int BufOp;
        public Vector2 BufTime;
        public Vector2 BufPow;

        [SerializeField] TextMeshProUGUI BufNameTx;
        [SerializeField] RawImage BufIcon;
        [SerializeField] RawImage BufOpIm;
        [SerializeField] Image BufFlame;


        [SerializeField] Image BufTimeFill;
        [SerializeField] TextMeshProUGUI BufPowTx;


        void LateUpdate()
        {
            var BufData = DB.Bufs.Find(x => (int)x.BufID == BufID);
            var FlameCol = Color.white;
            if (BufData != null)
            {
                ChangeText(BufNameTx,BufData.DispName);
                ChangeTexture(BufIcon,BufData.BufIcon, true);
                ChangeColor(BufIcon,BufData.IconCol);
            }
            else
            {
                ChangeText(BufNameTx,EnumToJp((Enum_Buf)BufID));
                ChangeColor(BufIcon, Color.clear);
            }
            if (BufOp == (int)Enum_BufOp.Non)
            {
                ChangeColor(BufOpIm,Color.clear);
                FlameCol = BufData != null ? BufData.FlameCol : Color.black;
            }
            else
            {
                var BufAdd = DB.BufOPs[BufOp];
                ChangeTexture(BufOpIm,BufAdd.OpIcon, true);
                ChangeColor(BufOpIm,Color.white);
                FlameCol = BufAdd.FlameCol;
            }
            FlameCol.a = 0.5f;
            ChangeColor(BufFlame, FlameCol);
            ChangeFill(BufTimeFill, 1f - (BufTime.x / Mathf.Max(1f, BufTime.y)));
            ChangeText(BufPowTx, ValueStrings(BufPow.x, true));
        }
    }
}
