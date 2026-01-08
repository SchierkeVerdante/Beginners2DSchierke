using UnityEngine;

public class ChangePlatformLayer : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == Layers.Player) {
            spriteRenderer.sortingLayerID = SortingLayer.NameToID("Default");
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == Layers.Player) {
            spriteRenderer.sortingLayerID = SortingLayer.NameToID("Levitating");
        }
    }
}
