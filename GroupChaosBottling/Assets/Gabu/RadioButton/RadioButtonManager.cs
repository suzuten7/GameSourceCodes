using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
public class RadioButtonManager : MonoBehaviour
{
    #region 変数

    [SerializeField]
    private List<RadioButtonSystem> buttons;
    private int buttonCount = 0;// 有効なボタンの数、ゼロオリジン
    public int ButtonCount { get; }
    private int currentIndex = 0;
    public delegate void OnClickButton(RadioButtonSystem button);

    #endregion

    #region 関数

    // currentIndexの値が変わったときに呼ばれるイベント
    public event Action<CurrentIndex> OnChangeIndex;


    /// <summary>
    /// 直接指定したボタンを選択する
    /// </summary>
    public void SelectIndex(int index)
    {
        buttons[currentIndex].ChangeStatu(UISystem_Gabu.AnimatorStatu.Normal);
        buttons[index].ChangeStatu(UISystem_Gabu.AnimatorStatu.Selected);
        currentIndex = index;

        // イベントを呼び出す
        var currentIndexData = new CurrentIndex(currentIndex);
        OnChangeIndex?.Invoke(currentIndexData);
    }

    /// <summary>
    /// 有効な次のボタンを選択する
    /// </summary>
    /// <param name="startIndex">ループの始まり、無限ループに入らないためだけに存在する</param>
    public void SelectNext(int startIndex, int index)
    {
        index++;
        index = (int)Mathf.Repeat(index, buttons.Count);
        if (index == startIndex)
        {
            return;
        }
        // 有効なボタンだった場合選択処理をし終了
        if (buttons[index].gameObject.activeSelf)
        {
            SelectIndex(index);
            return;
        }
        SelectNext(startIndex, index);
    }

    public void SelectNext()
    {
        SelectNext(currentIndex, currentIndex);
    }

    /// <summary>
    /// 有効な以前のボタンを選択する
    /// </summary>
    /// <param name="startIndex"></param>
    public void SelectPrevious(int startIndex, int index)
    {
        index = (int)Mathf.Repeat(--index, buttons.Count);
        if (index == startIndex)
        {
            return;
        }
        // 有効なボタンだった場合選択処理をし終了
        if (buttons[index].gameObject.activeSelf)
        {
            SelectIndex(index);
            return;
        }
        SelectPrevious(startIndex, index);
    }

    public void SelectPrevious()
    {
        SelectPrevious(currentIndex, currentIndex);
    }
    /// <summary>
    /// 次のボタンを有効にする
    /// </summary>
    public void SetEnableNext()
    {
        if (buttonCount >= buttons.Count)
        {
            Debug.LogWarning("ボタンの数が上限に達しています");
            return;
        }

        buttonCount++;
        buttons[buttonCount].gameObject.SetActive(true);
        buttons[buttonCount].ChangeStatu(UISystem_Gabu.AnimatorStatu.Normal);
        if (buttonCount > 0)
        {
            buttons[0].gameObject.SetActive(true);
            buttons[0].ChangeStatu(UISystem_Gabu.AnimatorStatu.Selected);
        }
    }

    /// <summary>
    /// 現在有効なボタンの内、最も後ろのボタンを無効にする
    /// </summary>
    public void SetDisableTail()
    {
        if (buttonCount <= 0)
        {
            Debug.LogWarning("ボタンの数が下限に達しています");
            return;
        }

        buttons[buttonCount].gameObject.SetActive(false);
        buttons[buttonCount].ChangeStatu(UISystem_Gabu.AnimatorStatu.Normal);
        buttonCount--;
        if (buttonCount == 0)
        {
            buttons[buttonCount].gameObject.SetActive(false);
            buttons[buttonCount].ChangeStatu(UISystem_Gabu.AnimatorStatu.Normal);
        }
    }

    /// <summary>
    /// 全てのボタンを無効にし、buttonCountを0にする
    /// </summary>
    public void ResetButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
        buttonCount = 0;
    }

    /// <summary>
    /// 指定した範囲のボタンを 有効/無効 にする
    /// </summary>
    /// <param name="headIndex">ここから</param>
    /// <param name="tailIndex">ここまで</param>
    /// <param name="isActive">有効/無効</param>
    private void SetActiveRange(int headIndex, int tailIndex, bool isActive)
    {
        int leftovers = 0;

        if (headIndex > buttons.Count)
        {
            Debug.LogWarning("範囲外を指定していたので、範囲内に納めました");
            headIndex = buttons.Count;
        }
        if (tailIndex < 0)
        {
            Debug.LogWarning("範囲外を指定していたので、範囲内に納めました");
            tailIndex = 0;
        }

        // 範囲が逆転している場合
        if (tailIndex < headIndex)
        {
            Debug.LogWarning("範囲が逆転していたので、一周回って実行させました");

            leftovers = tailIndex;
            headIndex = tailIndex;
            tailIndex = buttons.Count;
        }

        for (int i = headIndex; i < tailIndex; i++)
        {
            buttonCount = i;
            buttons[i].gameObject.SetActive(isActive);
            buttons[i].ChangeStatu(UISystem_Gabu.AnimatorStatu.Normal);
            if (i == currentIndex)
            {
                buttons[i].ChangeStatu(UISystem_Gabu.AnimatorStatu.Selected);
            }
        }

        // 選択中のボタンが無効になった場合、0番目のボタンを選択する
        if (!buttons[currentIndex].gameObject.activeSelf)
        {
            buttons[currentIndex].ChangeStatu(UISystem_Gabu.AnimatorStatu.Normal);
            buttons[0].ChangeStatu(UISystem_Gabu.AnimatorStatu.Selected);
            currentIndex = 0;
        }

        // 有効なボタンの数が0(1つ)の場合、ラジオボタンを無効にする
        if (buttonCount == 0)
        {
            buttons[0].ChangeStatu(UISystem_Gabu.AnimatorStatu.Selected);
            buttons[0].gameObject.SetActive(false);
        }

        // 範囲が逆転していた場合
        if (leftovers > 0)
        {
            SetActiveRange(0, leftovers, isActive);
        }
    }

    /// <summary>
    /// 0から指定した範囲までのボタンを有効にする
    /// </summary>
    /// <param name="range"></param>
    public void SetEableRange(int range)
    {
        SetActiveRange(0, range, true);
    }

    /// <summary>
    /// 指定した範囲のボタンを配列の後ろから順に無効にする
    /// </summary>
    /// <param name="range"></param>
    public void SetDisableRange(int range)
    {
        SetActiveRange(range, buttons.Count, false);
    }

    private void OnClick(int index)
    {
        SelectIndex(index);
    }

    public void OnClick(RadioButtonSystem button)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] == button)
            {
                OnClick(i);
                return;
            }
        }
        OnClick(0);
    }

    #endregion

    private void Awake()
    {
        if (buttons == null)
        {
            Debug.LogWarning("ボタンの設定がされていません");
            return;
        }

        ResetButtons();
        for (int i = 1; i < 20; i++)
        {
            buttons.Add(Instantiate(buttons[0], buttons[0].transform.parent));
        }
        foreach (var button in buttons)
        {
            button.buttons.onClick.AddListener(() =>
            {
                OnClick(button);
            });
        }
    }
    private void Start()
    {

    }
    //private IEnumerator Start()
    //{
    //    // 各関数を1秒おきにテスト
    //    yield return new WaitForSeconds(1f);
    //    Debug.Log("ResetButtons()");
    //    ResetButtons();

    //    yield return new WaitForSeconds(1f);
    //    Debug.Log($"SetEnableNext(){buttonCount}");
    //    SetEnableNext(); // ボタン1個有効

    //    yield return new WaitForSeconds(1f);
    //    Debug.Log($"SetEnableNext(){buttonCount}");
    //    SetEnableNext(); // ボタン2個有効

    //    yield return new WaitForSeconds(1f);
    //    Debug.Log("SetDisableTail()");
    //    SetDisableTail();

    //    yield return new WaitForSeconds(1f);
    //    Debug.Log("SetEableRange(5)");
    //    SetEableRange(10);

    //    yield return new WaitForSeconds(1f);
    //    Debug.Log("SetDisableRange(2)");
    //    SetDisableRange(4);

    //    yield return new WaitForSeconds(1f);
    //    Debug.Log("SelectNext()");
    //    SelectNext();

    //    yield return new WaitForSeconds(1f);
    //    Debug.Log("SelectNext()");
    //    SelectNext();

    //    yield return new WaitForSeconds(1f);
    //    Debug.Log("SelectNext()");
    //    SelectNext();

    //    yield return new WaitForSeconds(1f);
    //    Debug.Log("SelectPrevious()");
    //    SelectPrevious();

    //}
}


public class CurrentIndex
{
    public int index { get; }

    public CurrentIndex(int index)
    {
        this.index = index;
    }
}