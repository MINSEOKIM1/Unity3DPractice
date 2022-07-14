using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerBehavior _playerBehavior;
    void Start()
    {
        _playerBehavior = GetComponent<PlayerBehavior>();
    }
    
    void FixedUpdate()
    {
        var hValue = Input.GetAxisRaw("Horizontal");
        var vValue = Input.GetAxisRaw("Vertical");
        var spaceValue = Input.GetKeyDown(KeyCode.Space);
        var leftMouse = Input.GetKeyDown(KeyCode.Mouse0);

        _playerBehavior.InputProcessing(hValue, vValue, spaceValue, leftMouse);
    }
}
