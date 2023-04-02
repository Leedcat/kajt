using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ShowGrade : MonoBehaviour
{
    //public static ShowGrade instance;


    public GameObject picture;
    public float timePassed = 0f;
    public float interval = 3;
    public Sprite grade;
    public int gradedAttack;
    public ArrayList recentAttacks = new ArrayList { };


    private SpriteRenderer rend;
    private Sprite missedSprite, badSprite, goodSprite, perfectSprite;

    public ArrayList list;

    public float score = 4.0f;

    public int temp;
    // Start is called before the first frame update
    void Start()
    {

        // if (ShowGrade.instance == null){Debug.Log(instance);Debug.Log(ShowGrade.instance);ShowGrade.instance = this;
        missedSprite = Resources.Load<Sprite>("Missed");
        badSprite = Resources.Load<Sprite>("Bad");
        goodSprite = Resources.Load<Sprite>("Good");
        perfectSprite = Resources.Load<Sprite>("Perfect");
        rend = picture.GetComponent<SpriteRenderer>();

    list = new ArrayList { missedSprite, badSprite, goodSprite, perfectSprite};
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > interval/9)
        {
            int temp = (int)Math.Round(score);   // Round to closest whole number
            if (temp != 0)
            {
                temp -= 1;
            }
            grade = list[temp] as Sprite;
            showGrade(grade);
            
        }

        if (timePassed > interval / 9) 
        {
            gradedAttack = UnityEngine.Random.Range(0, 100); // Should be replaced by the actual graded attack
            if (recentAttacks.Count >= 2)  // Grade your 10 most recent attacks
            {
                recentAttacks.RemoveAt(0);
            }; 
            recentAttacks.Add(gradedAttack);
            score = 0;
            foreach (int i in recentAttacks)
            {
                score += i;
            };
            score /= 25;
            score /= recentAttacks.Count;
            interval += 3;
        }
    }

    public void showGrade(Sprite grades) 
    {
        Debug.Log("color is " + grades);
        rend.sprite = grades;
        var gObject = Instantiate(picture) as GameObject;

    }
    public static void showGrade()
    {
        Debug.Log("Final score is " + "");
    }
}
