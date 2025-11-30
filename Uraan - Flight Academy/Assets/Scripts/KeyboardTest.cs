using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardTest : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current == null)
            Debug.Log("Keyboard not detected ❌");

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            Debug.Log("SPACEBAR WORKS ✔");
    }
}
