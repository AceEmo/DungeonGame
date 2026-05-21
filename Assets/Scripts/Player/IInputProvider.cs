public interface IInputProvider
{
    float GetAxisRaw(string axisName);
    bool GetButtonDown(string buttonName);
    bool GetButton(string buttonName);   
}