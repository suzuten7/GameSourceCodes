using UnityEngine;
using UnityEngine.UI;

public class OniWaitSceneUI_Gabu : MonoBehaviour
{
    [SerializeField]
    private Texture[] _sprites;
    [SerializeField]
    private RawImage _rawImage;
    private int _index = 0;

    private void Update()
    {
        _index = (int)Mathf.Repeat(_index, _sprites.Length);
        _rawImage.texture = _sprites[_index];
    }

    public void IncrementIndex()
    {
        Debug.Log(_index);
        _index++;
        Debug.Log(_index);
    }
}
