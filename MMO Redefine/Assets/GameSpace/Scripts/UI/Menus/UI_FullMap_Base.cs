
namespace UIs
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Obj;
    using static GmSystem.GS_GlobalValues;
    using static Player.Player_MapObjSetter;
    using static GmSystem.GS_ChangeSet;
    public class UI_FullMap_Base : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI PlayerAreaTx;
        [SerializeField] TextMeshProUGUI PlayerPosTx;
        [SerializeField] Toggle NoPointTo;
        [SerializeField] TextMeshProUGUI LookPosTx;
        [SerializeField] Slider ZoomSlider;
        [SerializeField] float Hight;
        [SerializeField] float Scale;
        public float DragSpeed;
        public float MoveSpeed;

        [SerializeField] Vector2 PointSc;

        [SerializeField] List<UI_FullMap_Points> UIPoints;
        [SerializeField] List<TextMeshProUGUI> AreaTxs;

        [SerializeField] UI_FadeActive SelPointUI;
        [SerializeField] RawImage PointIcon;
        [SerializeField] TextMeshProUGUI PointName;
        [SerializeField] TextMeshProUGUI PointInfo;
        [SerializeField] GameObject ActionButtonObj;
        [SerializeField] TextMeshProUGUI ActionButtonTx;
        UI_FullMap_Points AUIPoint;

        GameObject mplayer = null;
        float ScaleF
        {
            get
            {
                return Mathf.Pow(2f,ZoomSlider.value);
            }
        }
        void LateUpdate()
        {
            if (MyPlayer!=null && mplayer != MyPlayer.gameObject)
            {
                mplayer = MyPlayer.gameObject;
                PosSetPlayer();
            }
            var PPos = MyPlayer.PosGet;
            ChangeText(PlayerAreaTx,AreaGet != null ? AreaGet.Name : "未定義エリア");
            ChangeText(PlayerPosTx,"プレイヤー座標\nx:" + PPos.x.ToString("F0") + "\ny:" + PPos.y.ToString("F0") + "\nz:" + PPos.z.ToString("F0"));

            var LPos = FullMapCamObj.transform.position;
            FullMapCamObj.orthographicSize = Scale / ScaleF;

            ChangeText(LookPosTx,"x:" + LPos.x.ToString("F0") + " z:" + LPos.z.ToString("F0"));

            var ObjPoints = FindObjectsByType<Obj_FullMapIcon>(FindObjectsSortMode.None);
            for(int i = 0; i < Mathf.Max(ObjPoints.Length, UIPoints.Count); i++)
            {
                if (UIPoints.Count <= i) UIPoints.Add(Instantiate(UIPoints[0], UIPoints[0].transform.parent));
                var up = UIPoints[i];
                if (i >= ObjPoints.Length)
                {
                    ChangeActive(up.gameObject, false);
                    continue;
                }
                if (NoPointTo.isOn)
                {
                    ChangeActive(up.gameObject, false);
                    continue;
                }
                var op = ObjPoints[i];
                ChangeActive(up.gameObject, true);
                var PLPos = new Vector3(op.transform.position.x - LPos.x, op.transform.position.z - LPos.z, 0);
                PLPos.x *= PointSc.x * ScaleF;
                PLPos.y *= PointSc.y * ScaleF;
                up.RTrans.localPosition = PLPos;
                up.ID = i;
                ChangeColor(up.OutImage, AUIPoint == up ? Color.yellow : Color.white);
                up.ObjMapIcon = op;
                ChangeTexture( up.Icon,op.Icon, true);
            }
            var AreaObjs = FindObjectsByType<Obj_AreaView>(FindObjectsSortMode.None);
            for (int i = 0; i < Mathf.Max(AreaObjs.Length, AreaTxs.Count); i++)
            {
                if (AreaTxs.Count <= i) AreaTxs.Add(Instantiate(AreaTxs[0], AreaTxs[0].transform.parent));
                var at = AreaTxs[i];
                if (i >= AreaObjs.Length)
                {
                    ChangeActive(at.gameObject, false);
                    continue;
                }
                if (NoPointTo.isOn)
                {
                    ChangeActive(at.gameObject, false);
                    continue;
                }
                var ao = AreaObjs[i];
                ChangeActive(at.gameObject, true);
                var PLPos = new Vector3(ao.transform.position.x + ao.MapOffSet.x - LPos.x, ao.transform.position.z + ao.MapOffSet.y - LPos.z, 0);
                PLPos.x *= PointSc.x * ScaleF;
                PLPos.y *= PointSc.y * ScaleF;
                at.rectTransform.localPosition = PLPos;
                ChangeText(at, ao.Name);
            }
        }
        public void Move(Vector2 Vect)
        {
            var LPos = FullMapCamObj.transform.position;
            LPos.y = Hight;
            LPos.x += Vect.x / ScaleF;
            LPos.z += Vect.y / ScaleF;
            FullMapCamObj.transform.position = LPos;
        }
        public void PosSetPlayer()
        {
            var LPos = MyPlayer.PosGet;
            LPos.y = Hight;
            FullMapCamObj.transform.position = LPos;
        }
        public void PointView(int ID)
        {
            if(!SelPointUI.isActive)SelPointUI.OpenClose();
            AUIPoint = UIPoints[ID];
            ChangeTexture(PointIcon,AUIPoint.ObjMapIcon.Icon, true);
            ChangeText(PointName,AUIPoint.ObjMapIcon.Name);
            ChangeText(PointInfo, AUIPoint.ObjMapIcon.Info);
            ChangeActive(ActionButtonObj, AUIPoint.ObjMapIcon.MapAction != null);
            if (AUIPoint.ObjMapIcon.MapAction != null)ChangeText(ActionButtonTx, AUIPoint.ObjMapIcon.MapAction.ActionName);
        }
        public void PointAction()
        {
            AUIPoint.ObjMapIcon.MapAction.Action();
        }
    }
}
