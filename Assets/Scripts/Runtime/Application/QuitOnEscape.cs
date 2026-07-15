using UnityEngine;
using UnityEngine.InputSystem;

public class QuitOnEscape : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
        }
    }
}