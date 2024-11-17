using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class KickScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> points;
    private int lastClickedPoint = -1;
    [SerializeField] private GameObject defaultCircle;
    [SerializeField] private List<GameObject> splines;
    [SerializeField] private GameObject defaultSpline;
    TextWriter tw;
    string filename = "";

    // Start is called before the first frame update
    void Start()
    {
        defaultCircle = GameObject.Find("Circle");
        defaultSpline = GameObject.Find("BezierPointTemplate");
        filename = Application.dataPath + "/test.txt";
        tw = new StreamWriter(filename, false);
    }

    //freezes all points besides idx
    public void FreezeAllExcept(int idx)
    {
        for (int i = 0; i < points.Count; i++)
        {
            if (i == idx) continue;
            points[i].GetComponent<Point>().SetFreeze(true);
        }
        lastClickedPoint = idx;
    }

    public void ChangeLastPointPosition(Vector2 newPos)
    {
        if(lastClickedPoint!=-1) points[lastClickedPoint].GetComponent<Point>().SetPosition(newPos);
    }

    public void ChangeLastPointPosition(Vector3 newPos)
    {
        if (lastClickedPoint != -1) points[lastClickedPoint].GetComponent<Point>().SetPosition(newPos);
    }

    public Vector2 LastPointPosition()
    {
        if (lastClickedPoint != -1)
        {
            return UDtoInches(points[lastClickedPoint].GetComponent<Point>().GetPosition());
        }
        return new Vector2(-1, -1);
    }

    //unfreezes all points
    public void UnfreezeAll()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].GetComponent<Point>().SetFreeze(false);
        }
    }

    public int GetSplineCount() { 
        return splines.Count; 
    }
    public Spline GetSpline(int idx)
    {
        return splines[idx].GetComponent<Spline>();
    }

    public float GetTotalArcLength()
    {
        float s = 0;
        foreach(GameObject spline in splines)
        {
            s += spline.GetComponent<Spline>().GetArcLength();
        }
        return s;
    }

    public Vector2 UDtoInches(Vector2 a)
    {
        Vector2 b = new Vector2();
        b.x = a.x + 13.38f;
        b.y = a.y + 13.23f;
        b *= 5.26905829596f;
        return b;
    }

    public Vector2 InchestoUD(Vector2 a)
    {
        Vector2 b = new Vector2();
        b = a/ 5.26905829596f;
        b.x -= 13.38f;
        b.y -= 13.23f;
        return b;
    }

    // Update is called once per frame
    void Update()
    {
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
            splines[splines.Count - 1].GetComponent<Spline>().SetPoint((splines.Count == 1 ? 0 : 3), points[points.Count - 1]);
            if (splines.Count > 1) splines[splines.Count - 1].GetComponent<Spline>().SetPoint(0, splines[splines.Count - 2].GetComponent<Spline>().GetPoint(3));
            for(int i = 1; i < (splines.Count == 1 ? 4 : 3); i++)
            {
                points.Add(Instantiate(defaultCircle));
                points[points.Count - 1].AddComponent<Point>();
                points[points.Count - 1].GetComponent<Point>().SetName("point " + (points.Count - 1));
                points[points.Count - 1].GetComponent<Point>().SetIndex(points.Count - 1);
                splines[splines.Count - 1].GetComponent<Spline>().SetPoint(i, points[points.Count - 1]);
            }
            if(splines.Count > 1)
            {
                GameObject point1 = splines[splines.Count - 2].GetComponent<Spline>().GetPoint(2);
                GameObject point2 = splines[splines.Count - 1].GetComponent<Spline>().GetPoint(1);
                point1.GetComponent<Point>().SetOpposite(point2);
                point2.GetComponent<Point>().SetOpposite(point1);
            }

            if (splines.Count == 1)
            {
                splines[0].GetComponent<Spline>().SetFirstSpline();
                splines[0].GetComponent<Spline>().SetLastSpline(true);
            }
            else
            {
                splines[splines.Count - 2].GetComponent<Spline>().SetLastSpline(false);
            }
            splines[splines.Count-1].GetComponent<Spline>().SetLastSpline(true);
            splines[splines.Count - 1].GetComponent<Spline>().FixToDefault();
            splines[splines.Count - 1].GetComponent<Spline>().complete = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            for(int i = 0; i < splines.Count; i++)
            {
                Debug.Log("pressing s");
                Spline ss = splines[i].GetComponent<Spline>();
                tw.WriteLine(UDtoInches(ss.GetPoint(0).GetComponent<Point>().GetPosition()) + " " + UDtoInches(ss.GetPoint(1).GetComponent<Point>().GetPosition()) + " " + UDtoInches(ss.GetPoint(2).GetComponent<Point>().GetPosition())+" "+ UDtoInches(ss.GetPoint(3).GetComponent<Point>().GetPosition()));
            }
            tw.Close();
        }
    }
    private void FixedUpdate()
    {
        //Debug.Log(GetTotalArcLength());
        if (splines.Count > 0)
        {
            //Debug.Log(splines[0].GetComponent<Spline>().FindNewPosition(0));
        }
    }
}
