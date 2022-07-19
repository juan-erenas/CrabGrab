using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCrab : MonoBehaviour
{

    [SerializeField] Camera mainCamera;
    public float MovementIncrement = 4f;
    public float ShrinkIncrement = 0.01f;

    private bool _StartMoving = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_StartMoving)
        {
            Move();
            Shrink();
            mainCamera.transform.position = transform.position + new Vector3(0, 0, -10);
        }
    }

    public void StartMoving()
    {
        _StartMoving = true;
    }

    private void Move()
    {
        var newPosition = new Vector3(0, MovementIncrement);
        transform.position += newPosition * Time.deltaTime;
    }

    private void Shrink()
    {
        var shrinkAmount = ShrinkIncrement * Time.deltaTime;
        transform.localScale -= new Vector3(shrinkAmount, shrinkAmount, shrinkAmount);
        if (transform.localScale.x <= 0.1f)
        {
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }


}
