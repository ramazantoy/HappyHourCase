using Interfaces;

public class JoystickInputProvider : IInputProvider {
    
    private readonly VariableJoystick joystick;

    public JoystickInputProvider(VariableJoystick joystick) {
        this.joystick = joystick;
    }

    public float GetHorizontal() {
        return joystick.Horizontal;
    }
}