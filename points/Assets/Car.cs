using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    bool followPath;
    const int fixedSecondsPerBezier = 3;
    double startTime;
    KickScript kickScript;

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
        }
    }
}
