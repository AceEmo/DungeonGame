using UnityEngine;

public static class InputProviderBootstrap
{
    public static IInputProvider EnsureInputProvider(GameObject gameObject)
    {
        IInputProvider inputProvider = gameObject.GetComponent<IInputProvider>();
        if (inputProvider == null)
        {
            inputProvider = gameObject.AddComponent<StandardInputProvider>();
        }

        return inputProvider;
    }
}
