using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spline : MonoBehaviour
{
    [SerializeField] private List<GameObject> points;
    [HideInInspector] private List<GameObject> curvePoints;
    [HideInInspector] private GameObject defaultSquare;
    [HideInInspector] private Vector2[,] linearPointsPosition, quadraticPointsPosition, cubicPointsPosition;
    [HideInInspector] private GameObject[,] linearPoints, quadraticPoints, cubicPoints;
    [HideInInspector] private bool[,] linearPointsCreated, quadraticPointsCreated, cubicPointsCreated;

    private int totalSteps = 100;
    public bool complete = false;

    // Start is called before the first frame update
    void Start()
    {
        defaultSquare = GameObject.Find("Square");
        for(int i = 0; i < 4; i++)
        {
            points.Add(new GameObject());
        }
        linearPointsPosition = new Vector2[3, totalSteps];
        quadraticPointsPosition = new Vector2[2,totalSteps];
        cubicPointsPosition = new Vector2[1,totalSteps];
        linearPoints = new GameObject[3, totalSteps];
        quadraticPoints = new GameObject[2, totalSteps];
        cubicPoints = new GameObject[1, totalSteps];
        linearPointsCreated = new bool[3, totalSteps];
        quadraticPointsCreated = new bool[2, totalSteps];
        cubicPointsCreated = new bool[1, totalSteps];
    }

    public void SetPoint(int x, GameObject point)
    {
        points[x] = point;
    }

    public void FixToDefault()
    {
        points[1].GetComponent<Point>().SetPosition(points[0].GetComponent<Point>().GetPosition()+new Vector2(2,2));
        points[2].GetComponent<Point>().SetPosition(points[0].GetComponent<Point>().GetPosition() + new Vector2(3, -2));
        points[3].GetComponent<Point>().SetPosition(points[0].GetComponent<Point>().GetPosition() + new Vector2(5, 0));
    }
    void FixedUpdate()
    {
        if (complete)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int t = 0; t < totalSteps; t++)
                {
                    float normalizedT = (float)t / (float)totalSteps;
                    Vector2 p0 = points[i].GetComponent<Point>().GetPosition();
                    Vector2 p1 = points[i + 1].GetComponent<Point>().GetPosition();
                    linearPointsPosition[i, t] = (normalizedT * p1 + (1 - normalizedT) * p0);
                    //if(linearPointsCreated[i,t]==false) linearPoints[i,t] = Instantiate(defaultSquare);
                    //linearPointsCreated[i,t] = true;
                    //linearPoints[i, t].GetComponent<Transform>().position = (normalizedT * p1 + (1 - normalizedT) * p0);
                }
            }
            for(int i = 0; i < 2; i++)
            {
                for (int t = 0; t < totalSteps; t++)
                {
                    float normalizedT = (float)t / (float)totalSteps;
                    Vector2 p0 = linearPointsPosition[i, t];
                    Vector2 p1 = linearPointsPosition[i+1, t];
                    quadraticPointsPosition[i, t] = (normalizedT * p1 + (1 - normalizedT) * p0);
                }
            }
            for(int i = 0; i < 1; i++)
            {
                for (int t = 0; t < totalSteps; t++)
                {
                    float normalizedT = (float)t / (float)totalSteps;
                    Vector2 p0 = quadraticPointsPosition[i, t];
                    Vector2 p1 = quadraticPointsPosition[i+1, t];
                    cubicPointsPosition[i, t] = (normalizedT * p1 + (1 - normalizedT) * p0);
                    if(cubicPointsCreated[i,t]==false) cubicPoints[i,t] = Instantiate(defaultSquare);
                    cubicPointsCreated[i,t] = true;
                    cubicPoints[i, t].GetComponent<Transform>().position = (normalizedT * p1 + (1 - normalizedT) * p0);
                }
            }
        }
    }
}
