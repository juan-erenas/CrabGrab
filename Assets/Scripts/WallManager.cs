using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{

    [SerializeField] GameObject ChildWall;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MakeHorizontalAnchor()
    {
        Vector2 position = new Vector2(0.5f, 0);
        ChildWall.transform.localPosition = position;
    }

    private void MakeVerticalAnchor()
    {
        Vector2 position = new Vector2(0, 0.5f);
        ChildWall.transform.localPosition = position;
    }

    public void SetName(string name)
    {
        ChildWall.name = name;
    }

    public void SetSize(float height, float width)
    {
        transform.localScale = new Vector3(width, height);

        if (width > height)
        {
            MakeHorizontalAnchor();

        } else if (height > width)
        {
            MakeVerticalAnchor();
        }
    }



}
