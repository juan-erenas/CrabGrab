using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceDisplay : MonoBehaviour
{
    public int _Value = 0;
    private SpriteRenderer _SpriteRenderer;

    public Die _Die;

    private bool _IsRollingDie = false;


    // Start is called before the first frame update
    void Start()
    {
        _SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RollDice()
    {
        _IsRollingDie = true;
        StartCoroutine(StartSwitchingFaces());
    }

    private IEnumerator StartSwitchingFaces()
    {
        float increment = 0.075f;

        do
        {
            SwitchFace();
            yield return new WaitForSeconds(increment);

        } while (_IsRollingDie);
    }

    public void SetFinalDieValue(int value)
    {
        _Value = value;
        StopRolling();
        _SpriteRenderer.sprite = _Die.GetSprite(value);
    }

    private void StopRolling()
    {
        _IsRollingDie = false;
    }

    private void SwitchFace()
    {
        var random = new System.Random();
        int randNum = random.Next(6);
        _SpriteRenderer.sprite = _Die.GetSprite(randNum + 1);
    }

    public bool EqualsValue(int value)
    {
        if (value == _Value) { return true; } else { return false; }
    }


}