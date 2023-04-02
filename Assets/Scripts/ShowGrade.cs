using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ShowGrade : MonoBehaviour
{
    public static ShowGrade instance;
    public GameObject picture;
    private Sprite missedSprite, badSprite, goodSprite, perfectSprite;
    [Header("Score Limits")]
    [field: SerializeField] private int perfectScoreLimit;
    [field: SerializeField] private int goodScoreLimit;
    [field: SerializeField] private int badScoreLimit;

    public ArrayList list;
    void Start()
    {

        if (ShowGrade.instance == null)
        {
            ShowGrade.instance = this;
        }

        missedSprite = Resources.Load<Sprite>("Missed");
        badSprite = Resources.Load<Sprite>("Bad");
        goodSprite = Resources.Load<Sprite>("Good");
        perfectSprite = Resources.Load<Sprite>("Perfect");

        list = new ArrayList { missedSprite, badSprite, goodSprite, perfectSprite };
    }
    public void ShowScore(int score)
    {
        Sprite sprite;
        if (score >= this.perfectScoreLimit)
        {
            sprite = this.perfectSprite;
        }
        else if (score >= this.goodScoreLimit)
        {
            sprite = this.goodSprite;
        }
        else if (score >= this.badScoreLimit)
        {
            sprite = this.badSprite;
        }
        else
        {
            sprite = this.missedSprite;
        }

        GameObject gObject = Instantiate(picture);
        gObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public static int CalculateScore(float attackTime, float walkingTime)
    {
        float attackMinTime = API.instance.attackTime;
        attackTime = Mathf.Max(attackTime, attackMinTime);
        walkingTime = Mathf.Min(walkingTime, attackTime);

        return (int)((walkingTime / attackTime) * (attackMinTime / attackTime) * 100);
    }
}
