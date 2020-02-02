using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Sprite start;
    public Sprite quit;

    private bool isStart;
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        isStart = true;
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart && image.sprite != start)
            image.sprite = start;
        else if (!isStart && image.sprite != quit)
            image.sprite = quit;

        if (Input.GetAxis("Vertical") > 0 && !isStart)
            isStart = true;
        else if (Input.GetAxis("Vertical") < 0 && isStart)
            isStart = false;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isStart)
                SceneManager.LoadScene(1);
            else
                Application.Quit();
        }
    }
}
