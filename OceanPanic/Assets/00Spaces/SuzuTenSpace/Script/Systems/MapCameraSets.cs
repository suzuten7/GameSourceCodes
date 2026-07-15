using UnityEngine;

public class MapCameraSets : MonoBehaviour
{
    [SerializeField] Transform MapPoss;
    [SerializeField] Transform MapBaseTrans;
    [SerializeField] Transform MapHightTrans;
    [SerializeField] float HightSet;
    Player_States MinePlayer;
    void LateUpdate()
    {
        if (MinePlayer == null)
        {
            foreach(var FPlayer in FindObjectsByType<Player_States>(FindObjectsSortMode.None))
            {
                if (FPlayer.photonView.IsMine)
                {
                    MinePlayer = FPlayer;
                    break;
                }
            }
        }
        if (MinePlayer != null)
        {
            Vector3 MapPosd = MapPoss.position;
            MapPosd.x = MinePlayer.Rig.position.x;
            MapPosd.z = MinePlayer.Rig.position.z;
            MapPoss.position = MapPosd;
            Vector3 HightPosd = MapHightTrans.position;
            HightPosd.y = Mathf.Min(MinePlayer.Rig.position.y + HightSet,MapBaseTrans.position.y);
            MapHightTrans.position = HightPosd;
        }
    }
}
