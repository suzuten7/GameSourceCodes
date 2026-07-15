using UnityEngine;

public class QuitManager : MonoBehaviour
{
    [SerializeField] private GameObject quitDialog;

    bool quitConfirmed = false;

    void OnEnable()
    { Application.wantsToQuit += OnWantsToQuit; }

    void OnDisable()
    { Application.wantsToQuit -= OnWantsToQuit; }

    bool OnWantsToQuit()
    {
        // 既に「はい」が押されているなら終了を許可
        if (quitConfirmed) { return true; }

        // 確認ダイアログを表示
        quitDialog.SetActive(true);

        // 一旦終了をキャンセル
        return false;
    }

    //// 「はい」ボタン
    //public void OnClickYes()
    //{
    //    quitConfirmed = true;
    //    Application.Quit();
    //}

    //// 「いいえ」ボタン
    //public void OnClickNo()
    //{
    //    quitDialog.SetActive(false);
    //}
}
