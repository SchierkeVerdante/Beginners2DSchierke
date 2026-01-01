using UnityEngine;
using UnityEngine.EventSystems;

public class ClickTest : MonoBehaviour, IPointerDownHandler {
    private void Start() {

        Camera cam = Camera.main;
        if (cam != null) {

            // Перевірка чи об'єкт в полі зору камери
            Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);

            if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1) {
                Debug.LogWarning("Object is outside camera viewport!");
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log($"CLICKED! {gameObject.name} at {Time.time}");
    }
}