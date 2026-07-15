using UnityEngine;
using static BackSceneStack;

public class BackScenePoper : MonoBehaviour
{
    public void Pop()
    {
        backSceneStack.Pop();
    }
}
