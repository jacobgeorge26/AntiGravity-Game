using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailSafe : MonoBehaviour
{
    public Rigidbody body; //player
    public GameObject target; //cursor target
    public Transform spawnPoint; //player reset transform

    //this is a failsafe script as a 'just in case' the player slips through any walls
    //they shouldn't but it's a pain for them if it does so this is a backup

    private Movement movement; //movement script
    public void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            Movement bodyMovement = body.GetComponent<Movement>();
            //allows collision to work - otherwise it locks aircontrol and messes with stuff when the trigger exits
            //if removed it causes the game to bug when the player is hit when 'stuck' to a wall
            bodyMovement.hit = true;
            bodyMovement.Reset(spawnPoint);

            //reset obstacles
            Obstacle resetObstacles = GameObject.Find("ObstacleManager").GetComponent<Obstacle>();
            resetObstacles.Reset();
        }
    }
}
