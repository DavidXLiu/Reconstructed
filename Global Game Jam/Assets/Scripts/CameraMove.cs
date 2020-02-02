using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    #region Inspector Variables
    public GameObject player;
    public Camera camera;
    public float lerpTime;
    public float cameraHeight;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Find camera if not set
        if (camera == null)
            camera = Camera.main;
        // Find player if not set
        if (player == null)
        {
            player = GameObject.FindGameObjectsWithTag("Player")[0];
        }
    }

    private void FixedUpdate()
    {
        float newXPos = Mathf.Lerp(transform.position.x, player.transform.position.x, lerpTime);
        float newYPos = Mathf.Lerp(transform.position.y, player.transform.position.y + cameraHeight, lerpTime);
        transform.position = new Vector3(newXPos, newYPos, -10);
    }

    // Update is called once per frame
    void Update()
    {
        /*float newXPos = Mathf.Lerp(transform.position.x, player.transform.position.x, lerpTime);
        float newYPos = Mathf.Lerp(transform.position.y, player.transform.position.y + cameraHeight, lerpTime);
        transform.position = new Vector3(newXPos, newYPos, -10);*/
    }
}
