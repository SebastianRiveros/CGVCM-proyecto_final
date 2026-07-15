using UnityEngine;

public class Flashlight : MonoBehaviour {
    Light light;

    void Start() {
        light = GetComponent<Light>();
        light.enabled = false;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F))
            light.enabled = !light.enabled;
    }
}