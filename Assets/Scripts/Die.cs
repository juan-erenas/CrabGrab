using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    private Dictionary<DieFaces, Sprite> _Sprites;

    public Sprite _Die1;
    public Sprite _Die2;
    public Sprite _Die3;
    public Sprite _Die4;
    public Sprite _Die5;
    public Sprite _Die6;

    private void Awake()
    {
        BuildTextures();
    }
    void Start()
    {
        BuildTextures();
    }

    private void BuildTextures()
    {
        _Sprites = new Dictionary<DieFaces, Sprite>();

        var spriteList = new List<Sprite>() { _Die1, _Die2, _Die3, _Die4, _Die5, _Die6 };


        DieFaces[] dieFaces = (DieFaces[]) Enum.GetValues(typeof(DieFaces));

        for (int i = 0; i < dieFaces.Length; i++)
        {
            var dieFace = dieFaces[i];

            _Sprites.Add(dieFace, spriteList[i]);
        }
    }

    public Sprite GetSprite(int value)
    {
        var dieFaces = (DieFaces[]) Enum.GetValues(typeof(DieFaces));

        var dieFace = dieFaces[value - 1];

        return _Sprites[dieFace];
    }

}

public enum DieFaces
{
    one,
    two,
    three,
    four,
    five,
    six
}
