using UnityEngine;
using Zenject;

public class InjectionTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"Injected gm : {gm}");
    }

    [Inject] private GameManager gm;
}
