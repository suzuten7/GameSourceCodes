using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class SESystem_Gabu : MonoBehaviour
{
    #region 変数

    [SerializeField]
    private AudioSource source;
    [SerializeField]
    private AudioResource[] resource;

    public float minVal = 1f;
    public float maxVal = 21f;

    float time = 0f;

    #endregion

    #region 関数

    private AudioResource LotteryMethod()
    {
        return resource[Random.Range(0, resource.Length)];
    }

    private AudioResource LotteryMethod(AudioResource currentResource)
    {
        int _i_NewResource = Random.Range(0, this.resource.Length);
        if (currentResource == resource[_i_NewResource])
        {
            _i_NewResource++;
        }

        return resource[_i_NewResource];
    }

    private bool OnVariableIsNull()
    {
        if (source != null)
        {
            return true;
        }
        if (resource != null)
        {
            return true;
        }

        return false;
    }

    #endregion

    private void Start()
    {
        if (OnVariableIsNull())
        {
            return;
        }

        time = Random.Range(minVal, maxVal);
        source.resource = LotteryMethod();
    }

    // Update is called once per frame
    void Update()
    {
        if (OnVariableIsNull())
        {
            return;
        }

        time -= Time.deltaTime;
        if (time < 0)
        {
            time = 0f;
            source.resource = LotteryMethod();
            source.Play();

            time = Random.Range(minVal, maxVal);
        }
    }
}
