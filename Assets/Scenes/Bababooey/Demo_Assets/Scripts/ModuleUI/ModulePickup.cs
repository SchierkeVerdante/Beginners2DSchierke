using UnityEngine;

public class ModulePickup : MonoBehaviour
{
    private void Awake() {
        gameObject.layer = Layers.Pickup;
    }
}
