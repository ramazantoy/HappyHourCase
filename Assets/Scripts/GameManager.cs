
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController _playerControllerPref;

    private void Start()
    {
        Instantiate(_playerControllerPref,new Vector3(0,0,-7),Quaternion.identity);
    }
}
