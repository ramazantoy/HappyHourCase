using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private VariableJoystick _joystick;
    
    
    public VariableJoystick Joystick => _joystick;

}
