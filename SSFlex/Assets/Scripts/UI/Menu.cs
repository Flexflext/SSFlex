using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    // Code: Haoke & Max

    public string menuName;
    public bool isOpen;

    // Enables UI
    public void Open()
    {
        isOpen = true;
        gameObject.SetActive(true);
    }

    // Disables UI
    public void Close()
    {
        isOpen = false;
        gameObject.SetActive(false);
    }
}
