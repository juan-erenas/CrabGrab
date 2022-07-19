using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    [SerializeField] Player PlayerOne;

    [SerializeField] TextMeshProUGUI ScoreLabel;
    [SerializeField] Room RoomPrefab;
    [SerializeField] GameObject Splat;

    private decimal _Score = 0;

    private Room _Room;

    private int gameStartWaitAmount = 2;
    private bool _ShouldSwipeRoom = false;
    private bool _ShouldShrinkRoom = false;

    public int tagCoolDown = 3;
    private float force = 1500;

    private System.Timers.Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        ScoreLabel.text = _Score.ToString();
        BuildRoom();

        PlayerOne.PlayerExploded += PlayerExploded;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerOne != null)
        {
            mainCamera.transform.position = PlayerOne.transform.position + new Vector3(0, 0, -10);
        }

        if (_ShouldSwipeRoom)
        {
            SwipeRoom();
        }

        if (_ShouldShrinkRoom)
        {
            ShrinkRoom();
        }
    }

    private IEnumerator RollDice(float movementSpeed)
    {

        yield return new WaitForSeconds(1);

        _Room.RollDice();

        yield return new WaitForSeconds(gameStartWaitAmount);

        _Room.StopRolling();
        _Room.StartCollapsing(movementSpeed);
    }

    public void PlayerExploded(object sender, EventArgs e)
    {
        EndGame();
    }

    private void EndGame()
    {
        var splat = Instantiate(Splat, PlayerOne.transform.position, Quaternion.identity);
        Destroy(PlayerOne.gameObject);
        _Room.StopCollapsing();
    }

    private void BuildRoom()
    {
        _Room = Instantiate(RoomPrefab, PlayerOne.transform.position, Quaternion.identity);
        _Room.GenerateRoom(15, 5, RoomType.UpperDoors, 5);
        _Room.PlayerExited += PlayerExited;

        _ShouldShrinkRoom = true;
        _Room.MasterObject.transform.localScale = new Vector3(2.5f, 2.5f);

        StartCoroutine(RollDice(1.2f));
    }

    private void ShrinkRoom()
    {
        float increment = 4f;
        _Room.MasterObject.transform.localScale += new Vector3(- increment * Time.deltaTime, - increment * Time.deltaTime, 1);

        if (_Room.MasterObject.transform.localScale.x <= 1)
        {
            _Room.MasterObject.transform.localScale = new Vector3(1, 1, 1);
            _ShouldShrinkRoom = false;
        }
    }

    public void PlayerExited(object sender, PlayerCrossedEventArgs e)
    {
        bool correctDoor = e.IsCorrectDoor;

        if (correctDoor)
        {
            AddPoint();
            SwipeRoomAway();
        }

        else
        {
            EndGame();
        }
    }

    private void AddPoint()
    {
        _Score += 0.5m;
        ScoreLabel.text = ((int)_Score).ToString();
    }

    private void SwipeRoomAway()
    {
        _ShouldSwipeRoom = true;
    }

    private void SwipeRoom()
    {
        float swipeSpeed = 10f;
        _Room.MasterObject.transform.position += new Vector3(0, - swipeSpeed * Time.deltaTime);

        if (_Room.MasterObject.transform.position.y < - 6)
        {
            ReplaceRoom();
        }
    }

    private void ReplaceRoom()
    {
        Destroy(_Room.MasterObject);
        Destroy(_Room);
        _Room = null;
        _ShouldSwipeRoom = false;
        //Add to Score???
        BuildRoom();
    }

    //private IEnumerator ShowCountDown()
    //{
    //    StartCoroutine(CollapseRoom(0.1f));
    //    RollDice();

    //    int secondsLeft = gameStartWaitAmount - 1;
    //    countDownLabel.text = @"" + gameStartWaitAmount;

    //    countDownLabel.gameObject.SetActive(true);

    //    while (secondsLeft != -1)
    //    {
    //        yield return new WaitForSeconds(1);

    //        countDownLabel.text = @"" + secondsLeft;

    //        secondsLeft--;
    //    }

    //    countDownLabel.gameObject.SetActive(false);
    //}




}
