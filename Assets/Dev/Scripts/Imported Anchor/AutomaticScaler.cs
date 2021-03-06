using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticScaler : MonoBehaviour
{
    public int originalWidth = 1080;
    public int originalHeight = 1920;

    public bool doUpdate;
    Vector3 originalScale;

    CameraAnchor CA;
    private void Start()
    {
        CA = GetComponent<CameraAnchor>();
        originalScale = transform.localScale;

        //if(transform.tag == "Clip" || transform.tag == "Board")
        //{
        //    CA.isMovePos = false;
        //}
        //else
        //{
        //    CA.isMovePos = true;
        //}

        Scaler();
    }

    private void Update()
    {
        if (doUpdate)
        {
            Scaler();
        }
    }
    public void Scaler()
    {
        float width = Screen.width;
        float height = Screen.height;

        float deltaWidth = originalWidth / width; 
        float deltaHeight = originalHeight / height;

        float actualDelta = 1;

        Vector3 newScale;

        if(deltaWidth == 1 && deltaHeight == 1)
        {
            CA.isMovePos = false;

            return;
        }

        CA.isMovePos = true;

        if (deltaWidth != 1)
        {
            actualDelta = deltaWidth;
        }
        else
        {
            actualDelta = deltaHeight;
        }

        newScale = new Vector3(originalScale.x * actualDelta, originalScale.y * actualDelta, originalScale.z);
        transform.localScale = newScale;
    }
}
