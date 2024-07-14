using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Point : MonoBehaviour
{
    float x=0, y=0;
    [HideInInspector] string waitSetName = "copy";
    float radius =  0.5f; //for the default white circle sprite
    [HideInInspector] bool leftClickPressed = false;
    int index = -1;
    bool freeze;
    GameObject oppositePoint = null; //the first and last points of 2 connecting beziers
    float magnitude; //from handle to connecting point
    Vector2 centerPointPosition;

    // Start is called before the first frame update
    void Start()
    {
        name = "copy";
        freeze = false;
    }

    public void SetName(string newName)
    {
        waitSetName = newName; //will change name in Update()
    }

    public string GetName()
    {
        return waitSetName;
    }

    public void SetPosition(Vector3 pos)
    {
        x = pos.x; 
        y = pos.y;
    }

    public void SetPosition(Vector2 pos)
    {
        x = pos.x;
        y = pos.y;
    }

    public void SetIndex(int idx)
    {
        index = idx;
    }

    public void SetFreeze(bool f)
    {
        freeze = f;
    }

    public Vector2 GetPosition()
    {
        return new Vector2(x, y);
    }

    public void SetOpposite(GameObject op)
    {
        oppositePoint = op;
        //Debug.Log(waitSetName + oppositePoint.GetComponent<Point>().GetName());
    }

    public GameObject GetOpposite()
    {
        return oppositePoint;
    }

    public void SetMagnitude(float m)
    { 
        magnitude = m;
    }

    public float GetMagnitude(){ 
        return magnitude;
    }

    public void SetCenterPointPosition(Vector2 c)
    {
        centerPointPosition = c;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Transform>().position = new Vector2(x, y);
        name = waitSetName;

        //look for drag
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool mouseOnCircle = Math.Pow((mouseWorldPos.x - x), 2) + Math.Pow((mouseWorldPos.y - y), 2) <= Math.Pow(radius, 2);
        if (Input.GetMouseButtonDown(0)&&mouseOnCircle&&!freeze)
        {
            leftClickPressed = true;
            Debug.Log("pressed " + name);
            GameObject.Find("GameObject").GetComponent<KickScript>().FreezeAllExcept(index);
        }
        else if (Input.GetMouseButton(0))
        {
            if (leftClickPressed&&!freeze)
            {
                SetPosition(mouseWorldPos);
                if (oppositePoint != null)
                {
                    float oldMagnitude = oppositePoint.GetComponent<Point>().GetMagnitude();
                    Vector2 newDirection = (centerPointPosition - new Vector2(mouseWorldPos.x, mouseWorldPos.y)).normalized;
                    oppositePoint.GetComponent<Point>().SetPosition(centerPointPosition + newDirection * oldMagnitude);
                }
                else Debug.Log("hellooooooo");
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            leftClickPressed=false;
            GameObject.Find("GameObject").GetComponent<KickScript>().UnfreezeAll();
        }
        else
        {
            leftClickPressed= false;
            GameObject.Find("GameObject").GetComponent<KickScript>().UnfreezeAll();
        }
    }
}
