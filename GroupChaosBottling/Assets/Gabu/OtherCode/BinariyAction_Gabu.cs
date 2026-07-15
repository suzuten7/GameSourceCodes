using UnityEngine;
using static UnityEngine.UI.Button;
using UnityEngine.Serialization;

public class BinariyAction_Gabu : MonoBehaviour
{

    // Event delegates triggered on click.
    [FormerlySerializedAs("onClick")]
    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

    public void UpdateAnimation(bool binary)
    {
        if (binary)
        {
            m_OnClick.Invoke();
        }
        else
        {
            m_OnClick.Invoke();
        }
    }
}
