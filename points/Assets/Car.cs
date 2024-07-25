using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    bool followPath;
    const int fixedSecondsPerBezier = 3;
    double startTime;
    KickScript kickScript;
    const float stepSize = 5; //want 5 per sec, fixedUpdate runs 50 times per sec, so 5 per 50 times, 0.1 per fixedupdate

    // Start is called before the first frame update
    void Start()
    {
        followPath = false;
        kickScript = GameObject.Find("GameObject").GetComponent<KickScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            followPath= true;
            startTime = Time.realtimeSinceStartupAsDouble;
        }
    }

    void FixedUpdate()
    {
        if (followPath)
        {
            /* fixed time per point
            double elapsedTime = Time.realtimeSinceStartupAsDouble - startTime;
            int currentBezier = (int)( elapsedTime / fixedSecondsPerBezier);
            if (currentBezier < kickScript.GetSplineCount())
            {
                int t = (int)((elapsedTime % fixedSecondsPerBezier)/fixedSecondsPerBezier * 100);
                Vector2 newPosition = kickScript.GetSpline(currentBezier).GetPointPosition(t);
                GetComponent<Transform>().position = newPosition;
            }
            if(elapsedTime> kickScript.GetSplineCount() * fixedSecondsPerBezier)
            {
                followPath = false;
            }
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
            //Debug.Log("" + totalLengthByNow + " " + totalMeasuredLength);
            //Debug.Log(overLength);
            if (currentBezier>=kickScript.GetSplineCount()) followPath = false;
            else
            {
                double overLength = kickScript.GetSpline(currentBezier).GetArcLength() - (totalMeasuredLength - totalLengthByNow);
                Vector2 newPosition = kickScript.GetSpline(currentBezier).FindNewPosition(overLength);
                GetComponent<Transform>().position = newPosition;
            }
        }
    }
}
