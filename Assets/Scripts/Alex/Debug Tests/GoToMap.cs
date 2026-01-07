using UnityEngine;
using Zenject;

public class GoToMap : MonoBehaviour
{
    [Inject] IGameManager gameManager;

    public void LoadMapScene() {
        gameManager.LoadMapScene();
    }
}
