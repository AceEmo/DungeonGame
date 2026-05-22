using UnityEngine;

public class MinimapInputHandler
{
    private readonly IInputProvider inputProvider;
    private bool isLargeMapOpen;

    public bool IsLargeMapOpen => isLargeMapOpen;

    public MinimapInputHandler(IInputProvider inputProvider)
    {
        this.inputProvider = inputProvider;
    }

    public bool ShouldToggleMap()
    {
        if (inputProvider.GetButtonDown("Map"))
        {
            isLargeMapOpen = !isLargeMapOpen;
            return true;
        }
        return false;
    }
}