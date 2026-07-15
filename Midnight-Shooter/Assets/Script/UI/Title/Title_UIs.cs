using UnityEngine;

/* 内容
 * ・タイトル内の各種UIのセット
 */

public class Title_UIs : MonoBehaviour
{
    [SerializeField] GameObject startUI;
    [SerializeField] GameObject setUI;
    [SerializeField] GameObject offlineUI;
    [SerializeField] GameObject onlineUI;


    void Start() { Back(); }

    /// <summary>
    /// オフラインのボタン
    /// </summary>
    public void Offline()
    {
        startUI.SetActive(false);
        setUI.SetActive(true);
        offlineUI.SetActive(true);
        onlineUI.SetActive(false);
    }

    /// <summary>
    /// オンラインのボタン
    /// </summary>
    public void Online()
    {
        startUI.SetActive(false);
        setUI.SetActive(true);
        offlineUI.SetActive(false);
        onlineUI.SetActive(true);
    }

    /// <summary>
    /// 戻るボタン(最初期)
    /// </summary>
    public void Back()
    {
        startUI.SetActive(true);
        setUI.SetActive(false);
        offlineUI.SetActive(false);
        onlineUI.SetActive(false);
    }
}
