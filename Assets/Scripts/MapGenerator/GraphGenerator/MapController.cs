using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [SerializeField] Spaceship2D spaceship;
    [SerializeField] StarMapGenerator generator;

    [SerializeField] Button startButton;
    private void Start() {
        if (startButton != null)
            startButton.onClick.AddListener(OnGenerateClicked);
    }

    private void OnGenerateClicked() {
        generator.GenerateMap();
    }

    private void OnDestroy() {
        if (startButton != null)
            startButton.onClick.RemoveListener(OnGenerateClicked);
    }
}


