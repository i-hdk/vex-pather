using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> points;
    [SerializeField] private GameObject defaultCircle;
    [SerializeField] private List<GameObject> splines;
    [SerializeField] private GameObject defaultSpline;

    // Start is called before the first frame update
    void Start()
    {
        defaultCircle = GameObject.Find("Circle");
        defaultSpline = GameObject.Find("BezierPointTemplate");
    }

    //freezes all points besides idx
    public void FreezeAllExcept(int idx)
    {
        for(int i = 0; i < points.Count; i++)
        {
            if (i == idx) continue;
            points[i].GetComponent<Point>().SetFreeze(true);
        }
    }

    //unfreezes all points
    public void UnfreezeAll()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].GetComponent<Point>().SetFreeze(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            points.Add(Instantiate(defaultCircle));
            points[points.Count - 1].AddComponent<Point>();
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            points[points.Count - 1].GetComponent<Point>().SetPosition(mouseWorldPos);
            points[points.Count - 1].GetComponent<Point>().SetName("point " + (points.Count-1));
            points[points.Count - 1].GetComponent<Point>().SetIndex(points.Count-1);
            //Debug.Log("pressed");
        }
        if (Input.GetKeyDown("space"))
        {
            points.Add(Instantiate(defaultCircle));
            points[points.Count - 1].AddComponent<Point>();
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            points[points.Count - 1].GetComponent<Point>().SetPosition(mouseWorldPos);
            points[points.Count - 1].GetComponent<Point>().SetName("point " + (points.Count - 1));
            points[points.Count - 1].GetComponent<Point>().SetIndex(points.Count - 1);

            splines.Add(Instantiate(defaultSpline));
            splines[splines.Count - 1].GetComponent<Spline>().SetPoint(0, points[points.Count - 1]);
            for(int i = 1; i < 4; i++)
            {
                points.Add(Instantiate(defaultCircle));
                points[points.Count - 1].AddComponent<Point>();
                points[points.Count - 1].GetComponent<Point>().SetName("point " + (points.Count - 1));
                points[points.Count - 1].GetComponent<Point>().SetIndex(points.Count - 1);
                splines[splines.Count - 1].GetComponent<Spline>().SetPoint(i, points[points.Count - 1]);
            }
            splines[splines.Count - 1].GetComponent<Spline>().FixToDefault();
            splines[splines.Count - 1].GetComponent<Spline>().complete = true;
        }
    }
}
