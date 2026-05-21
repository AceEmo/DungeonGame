public interface IInputProvider
{
    float GetAxisRaw(string axisName);
    bool GetButtonDown(string buttonName);
}