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

    public int totalSteps = 2000;
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

    public float FindNewRotation(double desiredLength)
    {
        double s = 0;
        for (int i = 1; i < totalSteps; i++)
        {
            //assuming each t is one step
            double dx = cubicPointsPosition[0, i].x - cubicPointsPosition[0, i - 1].x;
            double dy = cubicPointsPosition[0, i].y - cubicPointsPosition[0, i - 1].y;
            double integrand = Math.Sqrt(dx * dx + dy * dy);
            s += integrand;
            if (s >= desiredLength)
            {
                float t = (float)(i) / (float)(totalSteps);
                Vector2 p0 = points[0].GetComponent<Point>().GetPosition();
                Vector2 p1 = points[1].GetComponent<Point>().GetPosition();
                Vector2 p2 = points[2].GetComponent<Point>().GetPosition();
                Vector2 p3 = points[3].GetComponent<Point>().GetPosition();
                Vector2 term1 = 3 * ((1 - t) * (1 - t)) * (p1 - p0);
                Vector2 term2 = 6 * (1 - t) * t * (p2 - p1);
                Vector2 term3 = 3 * (t*t) * (p3 - p2);
                Vector2 derivative = term1 + term2 + term3;
                if (derivative.x == 0 && derivative.y > 0) return 0;
                else if (derivative.x == 0 && derivative.y < 0) return 180;
                return (float)(Math.Atan2(derivative.y, derivative.x) * 180.0 / Math.PI - 90.0);
            }
        }
        return 0;
    }

    public float GetPartialArcLength(double oldLength, double newLength)
    {
        if (oldLength > newLength) return 0;
        int oldT = 0;
        int newT = 0;
        double s = 0;
        for (int i = 1; i < totalSteps; i++)
        {
            //assuming each t is one step
            double dx = cubicPointsPosition[0, i].x - cubicPointsPosition[0, i - 1].x;
            double dy = cubicPointsPosition[0, i].y - cubicPointsPosition[0, i - 1].y;
            double integrand = Math.Sqrt(dx * dx + dy * dy);
            s += integrand;
            if (s >= oldLength)
            {
                oldT = i;
                break;
            }
        }
        s = 0;
        for (int i = 1; i < totalSteps; i++)
        {
            //assuming each t is one step
            double dx = cubicPointsPosition[0, i].x - cubicPointsPosition[0, i - 1].x;
            double dy = cubicPointsPosition[0, i].y - cubicPointsPosition[0, i - 1].y;
            double integrand = Math.Sqrt(dx * dx + dy * dy);
            s += integrand;
            if (s >= newLength)
            {
                newT = i;
                break;
            }
        }
        s = 0;
        for (int i = oldT; i <= newT; i++)
        {
            //assuming each t is one step
            double dx = cubicPointsPosition[0, i].x - cubicPointsPosition[0, i - 1].x;
            double dy = cubicPointsPosition[0, i].y - cubicPointsPosition[0, i - 1].y;
            double integrand = Math.Sqrt(dx * dx + dy * dy);
            s += integrand;
        }
        return (float)s;
    }

    public float GetCurvatureRadius(double desiredLength)
    {
        int p = 0;
        double s = 0;
        double dx, dy;
        for (int i = 1; i < totalSteps-2; i++)
        {
            //assuming each t is one step
            dx = cubicPointsPosition[0, i].x - cubicPointsPosition[0, i - 1].x;
            dy = cubicPointsPosition[0, i].y - cubicPointsPosition[0, i - 1].y;
            double integrand = Math.Sqrt(dx * dx + dy * dy);
            s += integrand;
            p = i;
            if (s >= desiredLength)
            {
                break;
            }
        }
        double dx1 = cubicPointsPosition[0, p+1].x - cubicPointsPosition[0, p].x;
        double dx2 = cubicPointsPosition[0, p + 2].x - cubicPointsPosition[0, p+1].x;
        double dy1 = cubicPointsPosition[0, p + 1].y - cubicPointsPosition[0, p].y;
        double dy2 = cubicPointsPosition[0, p + 2].y - cubicPointsPosition[0, p+1].y;
        dx = dx2 - dx1;
        dy = dy2 - dy1;
        return (float)(Math.Pow(dx1*dx1+dy1*dy1,1.5)/(dx*dy1-dy*dx1));

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
