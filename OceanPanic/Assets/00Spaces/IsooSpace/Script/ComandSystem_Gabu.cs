using UnityEngine;


public class ComandSystem_Gabu : MonoBehaviour
{
    //public PlayerInput[] playerInput;
    [TextArea]
    public string inputed = null;
    public string[] comands;
    public int i_comandNunber = 0;
    public int i_charaCounter = 0;
    public bool isComanding = false;

    private int i_handover = 100;


    private void Update()
    {
        if (inputed.Length >= 30000) // string型の最大文字数が約3万
        {
            inputed = GetTerminalCharacters(inputed, i_handover);
        }

        for (int i = 0; i < comands.Length; i++)
        {
            if (!inputed.EndsWith(comands[i]))
            {
                continue;
            }
            OnGetComand(GetTerminalCharacters(inputed, comands[i].Length));
        }
    }

    private void OnGetComand(string _comand)
    {
        switch (_comand)
        {
            case ",A":
                Debug.Log("成功！！");
                break;

            case ",ISOO":
                Debug.Log("意味のないコマンド");
                break;

            case ",TIME1/2":
                Time.timeScale /= 2;
                break;

            case ",TIMEshift;2":
                Time.timeScale *= 2;
                break;

            case ",CLOSE":
                Debug.Log("未実装");
                break;

            default: Debug.LogError("登録されていないコマンドを取得しました"); break;
        }
    }

    private string GetTerminalCharacters(string sentence, int quantity)
    {
        string a = null;
        for (int i = 0; i < quantity; i++)
        {
            a += sentence[sentence.Length - (quantity - i)];
        }
        return a;
    }
}


