using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    [SerializeField] WallManager WallManager;

    [SerializeField] DiceDisplay DiceDisplay;

    [SerializeField] InvisCollider InvisCollider;

    private Dictionary<string, WallManager> _SingleWalls;

    private Dictionary<string, List<WallManager>> _WallSets;

    private Dictionary<string, GameObject> _WallHolder;

    private List<InvisCollider> _DoorColliders;

    private Vector2 _UpperLeftCorner;
    private Vector2 _LowerLeftCorner;
    private Vector2 _LowerRightCorner;

    private List<DiceDisplay> DiceAboveDoors;
    private DiceDisplay DiceBelow;

    public float WallWidth = 1;
    public float DoorWidth = 3;
    private float _MovementIncrement = 2f;

    private int _NumOfDoors;
    private float _Width;
    private float _Height;
    private RoomType _RoomType;

    private float _WallOffset { get { return WallWidth / 2; } }

    public EventHandler<PlayerCrossedEventArgs> PlayerExited;


    private bool _IsCollapsing = false;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_IsCollapsing)
        {
            MoveWalls();
        }
    }

    public void GenerateRoom(float width, float height, RoomType roomType, int doorAmount)
    {
        this.name = "roomObject";
        _Height = height;
        _Width = width;
        _NumOfDoors = doorAmount;
        _RoomType = roomType;

        InstantiateWalls();
        BuildCorners();
        BuildDice();
        BuildRoom(); //TODO Add other room types
        BuildDoorColliders();
        AssignWallSets();
        
        
    }

    private void InstantiateWalls()
    {
        _SingleWalls = new Dictionary<string, WallManager>();

        var wallsList = new List<string>() { "left", "right", "upper", "lower" };
        var wallsToAdd = _RoomType switch
        {
            RoomType.UpperDoors => new List<bool>() { true, true, false, true },
            RoomType.LowerDoors => new List<bool>() { true, true, true, false },
            RoomType.RightDoors => new List<bool>() { true, false, true, true },
            RoomType.LeftDoors =>  new List<bool>() { false, true, true, true },

            _ => new List<bool>()
        };

        for (int i = 0; i < wallsList.Count; i++)
        {
            if (wallsToAdd[i] == true)
            {
                var newWall = Instantiate(WallManager, transform.position, Quaternion.identity);
                newWall.name = wallsList[i] + "_wall";
                _SingleWalls.Add(wallsList[i], newWall);
                newWall.transform.SetParent(this.transform);
            }
        }
    }

    private void BuildDice()
    {
        DiceAboveDoors = new List<DiceDisplay>();

        float distanceFromDoors = 1f;
        var diceAboveLocations = GetDiceLocations(distanceFromDoors);

        var diceBelowLocation = transform.position;
        diceBelowLocation += new Vector3(0, - _Height / 2 - distanceFromDoors, 0);

        for (int i = 0; i < diceAboveLocations.Count; i ++)
        {
            var newDie = Instantiate(DiceDisplay, diceAboveLocations[i], Quaternion.identity);
            newDie.name = $"Die_" + i.ToString();
            DiceAboveDoors.Add(newDie);
        }

        DiceBelow = Instantiate(DiceDisplay, diceBelowLocation, Quaternion.identity);
        DiceBelow.transform.SetParent(this.transform);
    }

    public void RollDice()
    {
        for (int i = 0; i < DiceAboveDoors.Count; i++)
        {
            DiceAboveDoors[i].RollDice();
        }

        DiceBelow.RollDice();
    }

    public void StopRolling()
    {
        var diceValues = GetDiceValues(DiceAboveDoors.Count);

        for (int i = 0; i < DiceAboveDoors.Count; i++)
        {
            DiceAboveDoors[i].SetFinalDieValue(diceValues[i]);
        }

        var RNG = new System.Random();
        var diceBelowValue = diceValues[RNG.Next(diceValues.Count)];

        DiceBelow.SetFinalDieValue(diceBelowValue);
    }

    private List<int> GetDiceValues(int amount)
    {
        var values = new HashSet<int>();
        var RNG = new System.Random();

        do
        {
            int randValue = RNG.Next(6);
            values.Add(randValue + 1);

        } while (values.Count < amount);

        return new List<int>(values);
    }

    private List<Vector2> GetDiceLocations(float distanceFromDoors)
    {
        var doorLocations = GetDoorLocations();

        Vector2 distanceFromDoor = new Vector2(0, distanceFromDoors);

        var diceLocations = new List<Vector2>();

        for (int i = 0; i < doorLocations.Count; i++)
        {
            doorLocations[i] += distanceFromDoor;
            diceLocations.Add(doorLocations[i]);
        }

        return diceLocations;
    }

    private void BuildDoorColliders()
    {
        _DoorColliders = new List<InvisCollider>();

        var doorLocations = GetDoorLocations();

        for(int i = 0; i < doorLocations.Count; i++)
        {
            var position = doorLocations[i] + new Vector2(0, WallWidth / 3 * 2);
            var invisCollider = Instantiate(InvisCollider, position, Quaternion.identity);
            var collider2d = invisCollider.GetComponent<Collider2D>();
            collider2d.transform.localScale = new Vector3(DoorWidth, WallWidth);
            invisCollider.PlayerCrossed += PlayerCrossed;
            invisCollider.DoorNum = i;

            _DoorColliders.Add(invisCollider);
        }
    }

    public void PlayerCrossed(object sender, PlayerCrossedEventArgs e)
    {
        int doorNum = e.DoorNum;

        var die = DiceAboveDoors[doorNum];

        if (die._Value == DiceBelow._Value)
        {
            CallPlayerCrossed(true);
        }

        else
        {
            CallPlayerCrossed(false); 
        }
    }

    private void CallPlayerCrossed(bool isCorrectDoor)
    {
        var eventArgs = new PlayerCrossedEventArgs();
        eventArgs.IsCorrectDoor = isCorrectDoor;
        var playerExited = PlayerExited;
        playerExited?.Invoke(this, eventArgs);
    }

    private List<Vector2> GetDoorLocations()
    {
        int numOfWalls = _NumOfDoors + 1;
        float wallLength = (_Width - (_NumOfDoors * DoorWidth)) / numOfWalls;
        float leftMostXPos = _UpperLeftCorner.x;

        float distanceToDoor = wallLength + (DoorWidth / 2);

        var doorLocations = new List<Vector2>();

        for (int i = 0; i < _NumOfDoors; i ++)
        {
            float xPos = leftMostXPos + distanceToDoor + ((distanceToDoor + (DoorWidth / 2)) * i);
            doorLocations.Add(new Vector2(xPos, _UpperLeftCorner.y));
        }

        return doorLocations;
    }

    public void StartCollapsing(float movementIncrement)
    {
        _MovementIncrement = movementIncrement;
        _IsCollapsing = true;
    }

    public void StopCollapsing()
    {
        _IsCollapsing = false;
    }

    private void MoveWalls()
    {
        var movementAmount = new Vector3(0, _MovementIncrement * Time.deltaTime, 0);

        var wallHolder = _WallHolder["upper"];

        wallHolder.transform.position -= movementAmount;

        foreach(string key in _SingleWalls.Keys)
        {
            if (key == "right" || key == "left")
            {
                _SingleWalls[key].transform.localScale -= movementAmount;
            }
        }
    }

    private void BuildCorners()
    {
        _UpperLeftCorner = new Vector2(-_Width / 2 + transform.position.x, _Height / 2 + transform.position.y);
        _LowerLeftCorner = new Vector2(-_Width / 2 + transform.position.x, -_Height / 2 + transform.position.y);
        _LowerRightCorner = new Vector2(_Width / 2 + transform.position.x, -_Height / 2 + transform.position.y);
    }

    private void BuildRoom() //TODO Add functionality to make other room types with doors facing other directions
    {
        _SingleWalls["left"].SetSize(_Height + (_WallOffset * 2), WallWidth);
        _SingleWalls["left"].SetName("left");
        _SingleWalls["right"].SetSize(_Height + (_WallOffset * 2), WallWidth);
        _SingleWalls["right"].SetName("right");
        _SingleWalls["lower"].SetSize(WallWidth, _Width + (_WallOffset * 2));
        _SingleWalls["lower"].SetName("lower");

        SetWallPositions();

        _WallSets = new Dictionary<string, List<WallManager>>();
        _WallSets.Add("upper", BuildHWallWithDoors(_NumOfDoors, _Width, "upper"));
    }

    private void AssignWallSets()
    {
        _WallHolder = new Dictionary<string, GameObject>();

        foreach(string key in _WallSets.Keys)
        {
            var newObject = Instantiate(new GameObject(), transform.position, Quaternion.identity);
            newObject.name = "Upper_Walls";

            var walls = _WallSets[key];

            for(int i = 0; i < walls.Count; i ++)
            {
                walls[i].transform.SetParent(newObject.transform);
            }

            for (int i = 0; i < DiceAboveDoors.Count; i++)
            {
                DiceAboveDoors[i].transform.SetParent(newObject.transform);
            }

            for (int i = 0; i < _DoorColliders.Count; i++)
            {
                _DoorColliders[i].transform.SetParent(newObject.transform);
            }

            _WallHolder.Add(key, newObject);
            newObject.transform.SetParent(this.transform);
        }
    }

    private void SetWallPositions()
    {
        _SingleWalls["left"].transform.position = new Vector2(_LowerLeftCorner.x, _LowerLeftCorner.y - _WallOffset);
        _SingleWalls["right"].transform.position = new Vector2(_LowerRightCorner.x, _LowerRightCorner.y - _WallOffset);
        //SingleWalls["upper"].transform.position = _UpperLeftCorner;
        _SingleWalls["lower"].transform.position = new Vector2(_LowerLeftCorner.x - _WallOffset, _LowerLeftCorner.y);
    }

    private List<WallManager> BuildHWallWithDoors(int doorNum, float width, string name)
    {
        var walls = new List<WallManager>();
        
        int numOfWalls = doorNum + 1;
        float wallLength = (width - (doorNum * DoorWidth)) / numOfWalls;

        for (int i = 0; i < numOfWalls; i ++)
        {
            var newWall = Instantiate(WallManager, transform.position, Quaternion.identity);
            newWall.SetSize(WallWidth, wallLength);
            newWall.SetName(name);
            walls.Add(newWall);
        }

        SetHWallsPositions(walls, wallLength);

        return walls;
    }

    private void SetHWallsPositions(List<WallManager> walls, float wallLength)
    {
        float yPos = _UpperLeftCorner.y;

        for (int i = 0; i < walls.Count; i ++)
        {
            float xPos = (wallLength + DoorWidth) * i + _UpperLeftCorner.x;
            
            walls[i].transform.position = new Vector2(xPos,yPos);
        }
    }

}
