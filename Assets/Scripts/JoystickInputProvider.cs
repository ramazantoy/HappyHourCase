using Interfaces;
using UnityEngine;


/// <summary>
/// Joystick paketinden gelen input'un dağıtılmasını sağlıyor interface'i aracılığıyla
/// </summary>
public class JoystickInputProvider : IInputProvider {
    
    private readonly VariableJoystick joystick;

    public JoystickInputProvider(VariableJoystick joystick) {
        this.joystick = joystick;
    }

    public Vector3 GetMovement() {
        return new Vector3(joystick.Horizontal, 0, joystick.Vertical);
    }

    public float GetHorizontalInput()
    {
        return joystick.Horizontal;
    }
}