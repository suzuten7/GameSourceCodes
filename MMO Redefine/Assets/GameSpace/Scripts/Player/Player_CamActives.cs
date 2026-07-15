
namespace Player
{
    using UnityEngine;
    using static UIs.UI_System;
    using static GmSystem.GS_ChangeSet;
    public class Player_CamActives : MonoBehaviour
    {
        [SerializeField] Camera CharaCam;
        [SerializeField] Camera FullMapCam;
        [SerializeField] Camera MainCam;
        [SerializeField] Camera PhotoCam;
        void Update()
        {
            ChangeActive(CharaCam.gameObject, ui_system.MyPlayerStateUI.isActive);
            ChangeActive(FullMapCam.gameObject, ui_system.FullMapFadeUI.isActive);
            ChangeActive(MainCam.gameObject, !PhotoMode);
            if (!PhotoCam.gameObject.activeSelf && PhotoMode)
            {
                PhotoCam.transform.position = MainCam.transform.position;
                PhotoCam.transform.rotation = MainCam.transform.rotation;
            }
            ChangeActive(PhotoCam.gameObject, PhotoMode);
        }
    }
}

