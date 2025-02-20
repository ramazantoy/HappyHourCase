

using UnityEngine;

namespace Interfaces
{
    
    /// <summary>
    /// Joystick paketinden gelen inputların ihtiyacımız olan yerlerde alınması için eklendi.
    /// Doğrudan class ile iletişim kurmak yerine sadece ilgili fonksiyonları kullanabilmek amacıyla.
    /// </summary>
    public interface IInputProvider {
        Vector3 GetMovement();
        
        float GetHorizontalInput();
    }
}