using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Car : MonoBehaviour
{
    //string filename = "";

    bool followPath;
    const int fixedSecondsPerBezier = 3;
    double startTime;
    KickScript kickScript;
    const float stepSize = 5; //want 5 per sec, fixedUpdate runs 50 times per sec, so 5 per 50 times, 0.1 per fixedupdate
    double prevOverLength = 0;
    //TextWriter tw;

    // Start is called before the first frame update
    void Start()
    {
        //filename = Application.dataPath + "/test.txt";
        followPath = false;
        kickScript = GameObject.Find("GameObject").GetComponent<KickScript>();
        //tw = new StreamWriter(filename, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            followPath= true;
            startTime = Time.realtimeSinceStartupAsDouble;
            prevOverLength = 0;
        }
    }

    void FixedUpdate()
    {
        if (followPath)
        {
            //log timestamp (seconds)
            //log radius of curvature(-r & r??), linear velocity (create new getArcLength function with current T to final T in this interval)
            /*
             * CW positve, if CCW need to negate!
             * in pros code: get radius of curvature for left & right by subtract/adding
             * V = Romega
             * VL = (R-w/2)*omega
            */
          
            double elapsedTime = Time.realtimeSinceStartupAsDouble - startTime;
            double totalLengthByNow = elapsedTime * stepSize;
            int currentBezier = 0;
            double totalMeasuredLength = 0;
            while(currentBezier< kickScript.GetSplineCount())
            {
                totalMeasuredLength += kickScript.GetSpline(currentBezier).GetArcLength();
                if (totalLengthByNow > totalMeasuredLength)
                {
                    currentBezier++;
                }
                else break;
            }

            if (currentBezier >= kickScript.GetSplineCount())
            {
                followPath = false;
                //tw.Close();
            }
            else
            {
                double overLength = kickScript.GetSpline(currentBezier).GetArcLength() - (totalMeasuredLength - totalLengthByNow);
                Vector2 newPosition = kickScript.GetSpline(currentBezier).FindNewPosition(overLength);
                GetComponent<Transform>().position = newPosition;
                double partialArc = kickScript.GetSpline(currentBezier).GetPartialArcLength(prevOverLength, overLength);
                double estLength;
                if (overLength > prevOverLength) estLength = (prevOverLength + overLength) / 2;
                else estLength = 0;
                double radiusOfCurvature = kickScript.GetSpline(currentBezier).GetCurvatureRadius(estLength);
                //Debug.Log(partialArc);
                //Debug.Log(radiusOfCurvature);
                prevOverLength = overLength;
                //tw.WriteLine(elapsedTime + " " + partialArc + " " + radiusOfCurvature);
            }
        }
    }
}
