using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spline : MonoBehaviour
{
    [SerializeField] private List<GameObject> points;
    [HideInInspector] private List<GameObject> curvePoints;
    [HideInInspector] private GameObject defaultSquare;
    [HideInInspector] private Vector2[,] linearPointsPosition;
    [HideInInspector] private GameObject[,] linearPoints;
    [HideInInspector] private bool[,] linearPointsCreated;

    private int totalSteps = 100;
    public bool complete = false;

    // Start is called before the first frame update
    void Start()
    {
        defaultSquare = GameObject.Find("Square");
        for(int i = 0; i < 7; i++)
        {
            points.Add(new GameObject());
        }
        linearPointsPosition = new Vector2[6, totalSteps];
        linearPoints = new GameObject[6, totalSteps];
        linearPointsCreated = new bool[6, totalSteps];
    }

    public void SetPoint(int x, GameObject point)
    {
        points[x] = point;
    }

    public void FixToDefault()
    {
        points[2].GetComponent<Point>().SetPosition(points[3].GetComponent<Point>().GetPosition()+new Vector2(-2,-2));
        points[4].GetComponent<Point>().SetPosition(points[3].GetComponent<Point>().GetPosition() + new Vector2(2, 2));
        points[1].GetComponent<Point>().SetPosition(points[3].GetComponent<Point>().GetPosition() + new Vector2(-3, 2));
        points[0].GetComponent<Point>().SetPosition(points[3].GetComponent<Point>().GetPosition() + new Vector2(-5, 0));
        points[5].GetComponent<Point>().SetPosition(points[3].GetComponent<Point>().GetPosition() + new Vector2(4, 0));
        points[6].GetComponent<Point>().SetPosition(points[3].GetComponent<Point>().GetPosition() + new Vector2(6, -2));
    }

    // Update is called once per frame
    void Update()
    {
        if(complete) for(int i = 0; i < 6; i++)
        {
            for(int t = 0; t < totalSteps; t++)
            {
                float normalizedT = (float)t/(float)totalSteps;
                Vector2 p0 = points[i].GetComponent<Point>().GetPosition();
                Vector2 p1 = points[i+1].GetComponent<Point>().GetPosition();
                linearPointsPosition[i,t] = (normalizedT * p1 + (1 - normalizedT) * p0);
                if(linearPointsCreated[i,t]==false) linearPoints[i,t] = Instantiate(defaultSquare);
                linearPointsCreated[i,t] = true;
                linearPoints[i, t].GetComponent<Transform>().position = (normalizedT * p1 + (1 - normalizedT) * p0);
            }
        }
    }
}
