using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    [Header("Mouse Input")]
    public Vector2 mouseInput;
    public bool panning;
    public bool leftMouse;
    public bool rightMouse;
    public bool openMenu;

    bool canCloseMenu = false;

    private void Start()
    {
        canCloseMenu = false;
    }

    public void MouseInput(InputAction.CallbackContext context)
    {
        mouseInput.x = Input.mousePosition.x;
        mouseInput.y = Input.mousePosition.y;   
    }

    public void Panning(InputAction.CallbackContext context)
    {
        panning = false;

        if (context.performed && !openMenu) { panning = true; }
    }

    public void LeftMouse(InputAction.CallbackContext context)
    {
        leftMouse = false;

        if (context.performed) { leftMouse = true; }
    }

    public void RightMouse(InputAction.CallbackContext context)
    {
        rightMouse = false;

        if (context.performed) { rightMouse = true; }
    }

    public bool MouseOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        return false;
    }

    public void OpenMenu(InputAction.CallbackContext context)
    {
        if (context.performed && !canCloseMenu)
        {
            openMenu = !openMenu;
            canCloseMenu = true;
        }

        if (context.canceled)
        {
            canCloseMenu = false;
        }
    }



}
