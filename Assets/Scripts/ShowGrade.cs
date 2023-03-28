using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGrade : MonoBehaviour
{
    public GameObject FloatingTextPrefab;
    public float timePassed = 0f;
    public int interval = 3;
    public string grade = "";
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
            grade = (interval/3).ToString();
            showgrade(grade);
            interval += 3;
        }
    }

    public void showgrade(string grades)
    {
        var gObject = Instantiate(FloatingTextPrefab) as GameObject;
        gObject.GetComponent<TextMesh>().text = grades;
    }
}
