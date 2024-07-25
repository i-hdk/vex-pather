using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spline : MonoBehaviour
{
    [SerializeField] private List<GameObject> points;
    [HideInInspector] private GameObject defaultSquare;
    [HideInInspector] private Vector2[,] linearPointsPosition, quadraticPointsPosition, cubicPointsPosition;
    [HideInInspector] private GameObject[,] cubicPoints;
    [HideInInspector] private bool[,] cubicPointsCreated;
    bool firstSpline, lastSpline;

    public int totalSteps = 100;
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
        cubicPoints = new GameObject[1, totalSteps];
        cubicPointsCreated = new bool[1, totalSteps];
        firstSpline = false;
    }

    public void SetPoint(int x, GameObject point)
    {
        points[x] = point;
    }

    public GameObject GetPoint(int x) { return points[x]; }

    public void SetFirstSpline()
    {
        firstSpline = true;
    }

    public void SetLastSpline(bool last)
    {
        lastSpline = last;
    }

    public void FixToDefault()
    {
        if (!firstSpline)
        {
            Vector2 otherSplinePointPos = points[1].GetComponent<Point>().GetOpposite().GetComponent<Point>().GetPosition();
            points[1].GetComponent<Point>().SetPosition(-otherSplinePointPos + 2*points[0].GetComponent<Point>().GetPosition());
            points[2].GetComponent<Point>().SetPosition(points[0].GetComponent<Point>().GetPosition() + new Vector2(3, -2));
            return;
        }
        points[1].GetComponent<Point>().SetPosition(points[0].GetComponent<Point>().GetPosition()+new Vector2(2,2));
        points[2].GetComponent<Point>().SetPosition(points[0].GetComponent<Point>().GetPosition() + new Vector2(3, -2));
        points[3].GetComponent<Point>().SetPosition(points[0].GetComponent<Point>().GetPosition() + new Vector2(5, 0));
    }
    public Vector2 GetPointPosition(int t)
    {
        return cubicPointsPosition[0,t];
    }

    //numerical integration w/ arc length of parametric curve formula: https://math.libretexts.org/Bookshelves/Calculus/Calculus_(OpenStax)/11%3A_Parametric_Equations_and_Polar_Coordinates/11.02%3A_Calculus_of_Parametric_Curves
    public float GetArcLength()
    {
        double s = 0;
        for(int i = 1; i < totalSteps; i++)
        {
            //assuming each t is one step
            double dx = cubicPointsPosition[0,i].x - cubicPointsPosition[0,i-1].x;
            double dy = cubicPointsPosition[0, i].y - cubicPointsPosition[0, i - 1].y;
            double integrand = Math.Sqrt(dx * dx + dy * dy);
            s += integrand;
        }
        return (float)s;
    }

    public Vector2 FindNewPosition(double desiredLength)
    {
        double s = 0;
        for(int i = 1; i < totalSteps; i++)
        {
            //assuming each t is one step
            double dx = cubicPointsPosition[0, i].x - cubicPointsPosition[0, i - 1].x;
            double dy = cubicPointsPosition[0, i].y - cubicPointsPosition[0, i - 1].y;
            double integrand = Math.Sqrt(dx * dx + dy * dy);
            s += integrand;
            if (s >= desiredLength)
            {
                return cubicPointsPosition[0,i];
            }
        }
        return new Vector2(0, 0);
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
            points[1].GetComponent<Point>().SetMagnitude((points[1].GetComponent<Point>().GetPosition() - points[0].GetComponent<Point>().GetPosition()).magnitude);
            points[2].GetComponent<Point>().SetMagnitude((points[3].GetComponent<Point>().GetPosition() - points[2].GetComponent<Point>().GetPosition()).magnitude);
            points[1].GetComponent<Point>().SetCenterPointPosition(points[0].GetComponent<Point>().GetPosition());
            points[2].GetComponent<Point>().SetCenterPointPosition(points[3].GetComponent<Point>().GetPosition());
        }
    }
}
