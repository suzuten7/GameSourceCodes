using System.Collections.Generic;
using UnityEngine;

public class Player_UI_DispMaker : MonoBehaviour
{
    [SerializeField] Transform PlayerTrans;
    [SerializeField] Camera Cam;
    [SerializeField] Canvas Canvasd;
    [SerializeField] bool Oni;
    [SerializeField] List<Player_UI_Sin_Marker> MarkerUIs;

    void Update()
    {
        var Markers = FindObjectsByType<MarkerObj>(FindObjectsSortMode.None);
        for (int i = 0; i < Mathf.Max(MarkerUIs.Count, Markers.Length); i++)
        {
            if (i < Markers.Length)
            {

                if (MarkerUIs.Count <= i)
                {
                    var CreUI = Instantiate(MarkerUIs[0], MarkerUIs[0].transform.parent);
                    MarkerUIs.Add(CreUI);
                }
                var SinUI = MarkerUIs[i];

                if (Markers[i].OniMarker && !Oni)
                {
                    MarkerUIs[i].gameObject.SetActive(false);
                    continue;
                }
                MarkerUIs[i].gameObject.SetActive(true);

                Color ArrowColor = Markers[i].MarkerColors;
                ArrowColor.a = 0.15f;
                SinUI.ArrowTx.color = ArrowColor;
                Color DistanceColor = Markers[i].MarkerColors * 0.75f;
                DistanceColor.a = 0.4f;
                SinUI.MarkerDisTx.color = DistanceColor;
                float Dis = Vector3.Distance(PlayerTrans.position, Markers[i].transform.position);
                SinUI.MarkerDisTx.text = Dis.ToString("F0") + "fm";
                #region 端表示
                float canvasScale = Canvasd.transform.localScale.z;
                var center = 0.5f * new Vector3(Screen.width, Screen.height);

                var VPos = Cam.WorldToViewportPoint(Markers[i].transform.position);
                bool Outs = VPos.z <= 0;
                if (VPos.x < 0f || VPos.x > 1f) Outs = true;
                if (VPos.y < 0f || VPos.y > 1f) Outs = true;
                SinUI.ArrowRtrans.gameObject.SetActive(Outs);
                var pos = Cam.WorldToScreenPoint(Markers[i].transform.position) - center;
                if (pos.z < 0f)
                {
                    pos.x = -pos.x;
                    pos.y = -pos.y;

                    if (Mathf.Approximately(pos.y, 0f))
                    {
                        pos.y = -center.y;
                    }
                }

                var halfSize = 0.5f * canvasScale * SinUI.BaseRTrans.sizeDelta;
                float d = Mathf.Max(
                    Mathf.Abs(pos.x / (center.x - halfSize.x)),
                    Mathf.Abs(pos.y / (center.y - halfSize.y))
                );
                bool isOffscreen = (pos.z < 0f || d > 1f);
                if (isOffscreen)
                {
                    pos.x /= d;
                    pos.y /= d;
                }
                pos *= 0.8f;
                SinUI.BaseRTrans.anchoredPosition = pos / canvasScale;
                if (isOffscreen)
                {
                    SinUI.ArrowRtrans.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg);
                }
                #endregion
            }
            else
            {
                MarkerUIs[i].gameObject.SetActive(false);
            }
        }
    }
}
