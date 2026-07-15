using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SuzutenDCre/StageData")]
public class Suzuten_StageData : ScriptableObject
{
    public string StageName;
    [TextArea]
    public string StageInfo;
    public int StageSceneID;
    public Sprite DrImage;
    public Texture BackImage;
    [Tooltip("ランダムで出現しない")]
    public bool RandomNoSelects;
}
