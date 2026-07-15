using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Suzuten_ActionData;
using static Suzuten_DataBase;
[CreateAssetMenu(menuName = "SuzutenDCre/ItemData")]
public class Suzuten_ItemData : ScriptableObject
{
    [TextArea]public string ItemMessage;
    public ParticleSystem GetEffect;
    [Tooltip("アイテム効果")]
    public ItemDsC[] ItemDs;

    [System.Serializable]
    public class ParametorsC
    {
        [Tooltip("対象パラメータ")]
        public ParametorE Parametor;
        [Tooltip("増減値")]
        public float Val;
        [Tooltip("割合%化")]
        public bool MaxPers;
    }

    [System.Serializable]
    public class ItemDsC
    {
        [Tooltip("効果対象")]
        public TargetsE Targets;
        [Tooltip("パラメータ")]
        public ParametorsC[] Parametors;
        [Tooltip("バフ")]
        public BufAddsC[] Bufs;
        [Tooltip("ワープ")]
        public TPsC[] TPs;
        [Tooltip("オブジェクト生成")]
        public GameObject[] InstObj;
    }
    [System.Serializable]
    public class TPsC
    {
        public TPsTargetE TPsTarget;
    }

    public enum TargetsE
    {
        使用者自身,
        使用者ターゲット,
        使用者以外,
        全員,
    }

    public enum TPsTargetE
    {
        使用者,
        使用者ターゲット,
        ランダム,
    }
}
