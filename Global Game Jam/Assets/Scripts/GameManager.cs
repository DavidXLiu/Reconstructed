using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;

        // Parts ignore collisions with players and enemies
        Physics2D.IgnoreLayerCollision(11, 9);
        Physics2D.IgnoreLayerCollision(11, 10);

        // Player Projectiles ignore collisions with player and parts and itself
        Physics2D.IgnoreLayerCollision(12, 9);
        Physics2D.IgnoreLayerCollision(12, 11);
        Physics2D.IgnoreLayerCollision(12, 12);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
