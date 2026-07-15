using UnityEngine;

public class ExitiController : MonoBehaviour
{
    [SerializeField] GameObject minCanvas;   // mainCanvaを保存
    [SerializeField] GameObject exitiCanvas;  // exitiCanvasを保存
    bool changeFlagg;  // 画面の状態(tureで確認画面、falseでタイトル画面)

    // exitiButtonを押した時の処理
    public void ExitiButton()
    {
        changeFlagg = true;
        ChangeCanvas();
    }

    // BackButtonを押した時の処理
    public void BackButton()
    {
        changeFlagg = false;
        ChangeCanvas();
    }

    // 画面を切り替える処理
    void ChangeCanvas()
    {
        if (changeFlagg == true)
        {
            minCanvas.SetActive(false);
            exitiCanvas.SetActive(true);
        }
        else
        {
            minCanvas.SetActive(true);
            exitiCanvas.SetActive(false);
        }
    }
}
