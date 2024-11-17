using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextControl : MonoBehaviour
{
    public TextMeshProUGUI output;
    public TMP_InputField newPosX, newPosY;

    public void ButtonDemo()
    {
        if(newPosX != null && newPosY != null) {
            Debug.Log("press detected and not null");
            Vector2 newPos = new Vector2(float.Parse(newPosX.text), float.Parse(newPosY.text));
            newPos = GameObject.Find("GameObject").GetComponent<KickScript>().InchestoUD(newPos);
            GameObject.Find("GameObject").GetComponent<KickScript>().ChangeLastPointPosition(newPos);
        }
    }

    public void FixedUpdate()
    {
        output.text = GameObject.Find("GameObject").GetComponent<KickScript>().LastPointPosition().ToString();
    }
}
