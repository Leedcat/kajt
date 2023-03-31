using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ShowGrade : MonoBehaviour
{
    public GameObject FloatingTextPrefab;
    public float timePassed = 0f;
    public float interval = 3;
    public string grade = "";
    public string[] list = {"bad","good","legendary"};
    public int gradedAttack;
    public ArrayList recentAttacks = new ArrayList{};

    public float score;

    public int temp;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > interval)
        {
            int temp = (int)Math.Round(score);   // Round to closest whole number
            grade = list[temp];
            showgrade(grade);
            interval += 3;
        }
        if (timePassed > interval/3)
        {
            gradedAttack = UnityEngine.Random.Range(0, 3); // should be replaced by the actual graded attack
            print(gradedAttack);
            if (recentAttacks.Count >= 5) {
                recentAttacks.RemoveAt(0);
            } ;
            recentAttacks.Add(gradedAttack);
            score = 0;
            foreach (int i in recentAttacks)
            {
                score += i;
            };
            score /= recentAttacks.Count;
            score -= score % 1;
        }
    }

    public void showgrade(string grades)
    {
        var gObject = Instantiate(FloatingTextPrefab) as GameObject;
        gObject.GetComponent<TextMesh>().text = grades;
    }
}
