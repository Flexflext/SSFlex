using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] Menu[] menus;

    private void Awake()
    {
        Instance = this;
    }


    // Takes a string and open its menu.
    public void OpenMenu(string _menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == _menuName)
            {
                OpenMenu(menus[i]);
            }
            else if (menus[i].isOpen)
            {
                CloseMenu(menus[i]);
            }
        }
    }

    // Takes a menu script and opens its menu.
    public void OpenMenu(Menu _menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].isOpen)
            {
                CloseMenu(menus[i]);
            }
        }

        _menu.Open();
    }

    public void CloseMenu(Menu _menu)
    {
        _menu.Close();
    }
}
