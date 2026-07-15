
namespace Player
{
    using UIs;
    using UnityEngine;
    using UnityEngine.UI;
    using Obj;
    using static FNet.Fusion_Manager;
    using static Datas.Data_Get;
    using static GmSystem.GS_ChangeSet;
    public class Player_MapObjSetter : MonoBehaviour
    {
        static public Camera FullMapCamObj;
        [SerializeField] Player_State PSta;
        [SerializeField] Camera FullMapCam;
        [SerializeField] Camera MMapMainCam;
        [SerializeField] Camera MMapIconCam;

        [SerializeField] RawImage MMIconImage;
        [SerializeField] Obj_FullMapIcon FMapObj;

        void Update()
        {
            if (CanControl(PSta.Object) && PSta.BotID < 0)
            {
                FullMapCamObj = FullMapCam;

                var Pos = PSta.PosGet;
                Pos.y = 1500;
                MMapMainCam.transform.position = Pos;
                MMapIconCam.orthographicSize = MMapMainCam.orthographicSize;
            }
            var Icon = PSta.ModelSet.PlayerIconGet(out _);
            ChangeTexture(MMIconImage, Icon, true);
            FMapObj.Icon = Icon;
        }
    }
}
