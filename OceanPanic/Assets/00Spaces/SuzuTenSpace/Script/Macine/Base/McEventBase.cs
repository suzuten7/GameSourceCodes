using UnityEngine;

public class McEventBase : MonoBehaviour
{
    [SerializeField]protected string EventMessage;
    virtual public void McEvent() { }
}
