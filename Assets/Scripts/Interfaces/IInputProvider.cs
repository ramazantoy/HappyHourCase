

using UnityEngine;

namespace Interfaces
{
    public interface IInputProvider {
        Vector3 GetMovement();
        
        float GetHorizontalInput();
    }
}