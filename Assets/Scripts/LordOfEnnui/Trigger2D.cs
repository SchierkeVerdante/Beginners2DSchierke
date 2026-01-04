using UnityEngine;
using UnityEngine.Events;

public class Trigger2D : MonoBehaviour
{
    public bool triggerActive, playerInside;

    public UnityEvent<GameObject, bool> triggerEvent;

    SpriteRenderer spriteRenderer;
    [SerializeField]
    Collider2D collider2d;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == Layers.Player && triggerActive) {
            playerInside = true;
            triggerEvent.Invoke(collision.gameObject, playerInside);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == Layers.Player && triggerActive) {
            playerInside = false;
            triggerEvent.Invoke(collision.gameObject, playerInside);
        }
    }

    public void SetActive(bool active) {
        triggerActive = active;
        if (spriteRenderer != null) spriteRenderer.color = triggerActive ? playerInside ? Color.blue : Color.green : Color.red;
        OnValidate();
    }

    private void OnValidate() {
        if (collider2d != null) collider2d.enabled = triggerActive;
    }
}
