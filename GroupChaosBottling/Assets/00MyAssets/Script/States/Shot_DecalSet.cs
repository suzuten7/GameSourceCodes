using UnityEngine;

public class Shot_DecalSet : MonoBehaviour
{
    [SerializeField]float Hight;
    private void LateUpdate()
    {
        transform.eulerAngles = new Vector3(90, 0, 0);
        transform.localPosition = Vector3.zero;
        transform.position += Vector3.up * Hight;
    }
}
