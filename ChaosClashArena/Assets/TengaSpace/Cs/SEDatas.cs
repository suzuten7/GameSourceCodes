using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SuzutenDCre/SEDatas")]
public class SEDatas : ScriptableObject
{
    public AudioClip[] AudioClips;
    public SEDataAdd[] SEAdds;
    static public Dictionary<string, Vector2Int> SEDic = new Dictionary<string, Vector2Int>();
    static public SEDatas SEData;
    public void SEDic_Set()
    {
        SEData = this;
        SEDic.Clear();
        for (int i = 0; i < AudioClips.Length; i++)
        {
            var Aucp = AudioClips[i];
            SEDic.TryAdd(Aucp.name, new Vector2Int(-1,i));
        }
        for(int i = 0; i < SEAdds.Length; i++)
        {
            for(int j = 0; j < SEAdds[i].AudioClips.Length; j++)
            {
                var Aucp = SEAdds[i].AudioClips[j];
                SEDic.TryAdd(Aucp.name, new Vector2Int(i,j));
            }
        }
    }
    static public Vector2Int SEIDGet(string SEname)
    {
        if (SEDic.TryGetValue(SEname, out var v)) return v;
        else return new Vector2Int(-2, 0);
    }
    static public AudioClip SEGetID(Vector2Int SEID)
    {
        if (SEID.x >= 0) return SEData.SEAdds[SEID.x].AudioClips[SEID.y];
        else return SEData.AudioClips[SEID.y];
    }
}
