using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    float movementIncrement = 3f;
    float dashIncrement = 10f;
    float rotationSpeed = 1000f;

    bool isDashing = false;
    bool canDash = true;
    bool canMove = true;

    Vector3 dashPosChange = new Vector3();

    float dashCooldown = 0.5f;
    float dashDuration = 0.15f;


    public KeyCode upButton = KeyCode.W;
    public KeyCode downButton = KeyCode.S;
    public KeyCode leftButton = KeyCode.A;
    public KeyCode rightButton = KeyCode.D;

    bool shouldMoveUp = false;
    bool shouldMoveRight = false;
    bool shouldMoveLeft = false;
    bool shouldMoveDown = false;

    // Update is called once per frame
    void Update()
    {
        if (canMove == false) { return; }

        if (isDashing) {
            Dash();
            return;
        }

        CheckIfShouldMove();
        CheckIfDashed();
    }

    private void CheckIfShouldMove()
    {
        Vector3 position = new Vector3();

        if (Input.GetKey(leftButton))
            position.x -= movementIncrement;

        if (Input.GetKey(rightButton))
            position.x += movementIncrement;

        if (Input.GetKey(upButton))
            position.y += movementIncrement;

        if (Input.GetKey(downButton))
            position.y -= movementIncrement;


        transform.position += position * Time.deltaTime;

        if (position != new Vector3(0, 0, 0))
        {
            Vector2 moveDirection = new Vector2(position.x, position.y);
            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }


    private void CheckIfDashed()
    {
        if (Input.GetKey(KeyCode.Space) && canDash)
        {
            float xPosChange = 0;
            float yPosChange = 0;


            if (Input.GetKey(rightButton))
                xPosChange += dashIncrement;

            if (Input.GetKey(leftButton))
                xPosChange -= dashIncrement;

            if (Input.GetKey(upButton))
                yPosChange += dashIncrement;

            if (Input.GetKey(downButton))
                yPosChange -= dashIncrement;

            dashPosChange = new Vector3(xPosChange, yPosChange, 0);

            Dash();
            
            if (isDashing == false)
            {
                StartCoroutine(StartDashCountdown());
                isDashing = true;
            }
        }
    }

    private void Dash()
    {
        this.transform.position += dashPosChange * Time.deltaTime;
    }

    IEnumerator StartDashCountdown()
    {
        yield return new WaitForSeconds(dashDuration);

        canDash = false;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    public void Immobolize()
    {
        StartCoroutine(makeImmobile(3));
    }

    private IEnumerator makeImmobile(int seconds)
    {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }


}
