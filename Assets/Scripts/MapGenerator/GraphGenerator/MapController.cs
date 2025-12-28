using System;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [SerializeField] Spaceship2D spaceship;
    [SerializeField] GraphGenerator graphGen;

    [SerializeField] Button startButton;
    private void Start() {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);
    }

    private void OnStartClicked() {
        graphGen.GenerateGraph();
    }

    private void OnDestroy() {
        if (startButton != null)
            startButton.onClick.RemoveListener(OnStartClicked);
    }
}
