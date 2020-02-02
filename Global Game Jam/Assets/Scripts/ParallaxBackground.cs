using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public float speed;

    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x + (Input.GetAxis("Horizontal") * speed), rectTransform.anchoredPosition.y);
        if (rectTransform.anchoredPosition.x < -927)
            rectTransform.anchoredPosition = new Vector3(1122, rectTransform.anchoredPosition.y, 0);
        else if (rectTransform.anchoredPosition.x > 1122)
            rectTransform.anchoredPosition = new Vector3(-927, rectTransform.anchoredPosition.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Input.GetAxis("Horizontal") * speed, 0, 0);

        /*rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x + (Input.GetAxis("Horizontal") * speed), rectTransform.anchoredPosition.y);
        if (rectTransform.anchoredPosition.x < -927)
            rectTransform.anchoredPosition = new Vector3(1122, rectTransform.anchoredPosition.y, 0);
        else if (rectTransform.anchoredPosition.x > 1122)
            rectTransform.anchoredPosition = new Vector3(-927, rectTransform.anchoredPosition.y, 0);*/
    }
}
