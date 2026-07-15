using UnityEngine;
using UnityEngine.UI;

/* 内容
 * ・画像のスクロールをする
 */

#region 変数倉庫1
[System.Serializable]
class BG_ScrollList
{
    public RawImage rawImage;
    public Vector2 offSet;
    [Tooltip("1週の速度倍率")]
    public Vector2 speed;
}
#endregion

public class Anime_ScrollImage : MonoBehaviour
{
    #region 変数倉庫2
    #region メインオプション
    [Header("◆メインオプション")]
    [SerializeField] Vector2 baseSpeed = new Vector2(1f, 1f);
    [SerializeField] BG_ScrollList[] scrollList;
    float timer = 0;
    #endregion
    #endregion

    void Update()
    {
        timer += Time.deltaTime;

        foreach (var scr in scrollList)
        {
            var rect = scr.rawImage.uvRect;
            rect.x = scr.offSet.x + timer * baseSpeed.x * scr.speed.x;
            rect.y = scr.offSet.y + timer * baseSpeed.y * scr.speed.y;
            scr.rawImage.uvRect = rect;
        }
    }
}
