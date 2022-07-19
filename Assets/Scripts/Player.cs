using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public EventHandler PlayerExploded;

    private bool _IsTouchingUpperWall = false;
    private bool _IsTouchingLowerWall = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        UpdateIsTouching(collision.gameObject.name, true);
        IsTouchingBothWalls();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        UpdateIsTouching(collision.gameObject.name, false);
        IsTouchingBothWalls();
    }

    private void UpdateIsTouching(string nameOfObject, bool isTouching)
    {
        if (nameOfObject == "upper")
        {
            _IsTouchingUpperWall = isTouching;
        }

        else if (nameOfObject == "lower")
        {
            _IsTouchingLowerWall = isTouching;
        }
    }

    private void IsTouchingBothWalls()
    {
        if (_IsTouchingLowerWall && _IsTouchingUpperWall)
        {
            ExplodePlayer();
        }
    }

    private void ExplodePlayer()
    {
        EventArgs eventArgs = new EventArgs();
        var playerExploded = PlayerExploded;
        playerExploded?.Invoke(this, eventArgs);
    }


}
