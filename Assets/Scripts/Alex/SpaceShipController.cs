using UnityEngine;
using Zenject;

public class SpaceShipController : MonoBehaviour {
    public Spaceship2D spaceship;
    [Inject] private InputManager _inputManager;
    InputSystem_Actions.UIActions uiActions;
    private void Start() {
        if (spaceship == null) {
            spaceship = GetComponent<Spaceship2D>();
        }
        uiActions = _inputManager.InputActions.UI;
        uiActions.Click.performed += ctx => OnClickPerformed();
    }

    private void OnClickPerformed() {
        Vector2 mousePosition = uiActions.MousePosition.ReadValue<Vector2>();
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        SetTargetPoint(worldPosition);
    }

    private void SetTargetPoint(Vector3 point) {
        if (spaceship != null) {
            spaceship.SetTarget(point);
        }
    }
}