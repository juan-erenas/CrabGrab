using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisCollider : MonoBehaviour
{

    public int DoorNum;
    public EventHandler<PlayerCrossedEventArgs> PlayerCrossed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "PlayerOne")
        {
            var eventArgs = new PlayerCrossedEventArgs();
            eventArgs.DoorNum = DoorNum;
            var playerCrossed = PlayerCrossed;
            playerCrossed?.Invoke(this, eventArgs);
        }
    }

}

public class PlayerCrossedEventArgs : EventArgs
{
    public int DoorNum { get; set; }
    public bool IsCorrectDoor { get; set; }
}
