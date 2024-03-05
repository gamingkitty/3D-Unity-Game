using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaBarScript : MonoBehaviour
{
    public bool isVisible = false;
    public float stamina = 100;
    public float maxStamina = 100;

    float scaleY;
    float scaleX;
    float scaleZ;
    // Start is called before the first frame update
    void Start()
    {
        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
        scaleZ = transform.localScale.z;
    }

    // Update is called once per frame
    void Update()
    {
        //If stamina bar is on screen, update its visuals with current stamina values
        if (isVisible)
        {
            transform.localScale = new Vector3(stamina / maxStamina, scaleY, scaleZ);
        }
        //Hide it if not visible
        else
        {
            transform.localScale = Vector3.zero;
        }
    }
}
