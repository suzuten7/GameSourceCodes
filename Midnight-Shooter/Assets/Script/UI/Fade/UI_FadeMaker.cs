using UnityEngine;

/* 内容
 * ・フェードの生成とドントデストロイ化
 * ・すでにあるときは生成しない
 */

public class UI_FadeMaker : MonoBehaviour
{
    [SerializeField] GameObject fade_Prefab;
    static GameObject cloneF_Obj;

    void Awake()
    {
        if (cloneF_Obj != null) return;

        cloneF_Obj = Instantiate(fade_Prefab);
        DontDestroyOnLoad(cloneF_Obj);
    }
}
