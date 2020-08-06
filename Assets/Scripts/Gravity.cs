using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public Rigidbody body; //player
    public GameObject target; //cursor target
    public GameObject wall; //wall - tags are "North" and "South"

    //this script turns gravity on/off depending on whether the player is in a corridor or in the main room 
    //allows for extension to more stuff outside room
    //or a different movement script to allow the player to 'walk'

    public void OnTriggerEnter(Collider collision)
    {
        //turn gravity on (the Collision script turns it off and the boxcolliders are right next to each other
            //-this could be made smoother as right now the player has to move forward a few times before they're in the room properly)
        //also formatting stuff with the target, making sure aircontrol is on
        var targetRenderer = target.GetComponent<Renderer>();
        targetRenderer.material.SetColor("_Color", Color.red);
        body.GetComponent<Movement>().airControl = true;
        body.velocity = Vector3.zero;
        body.useGravity = true;
    }

    public void OnTriggerExit(Collider coll)
    {
        //this is a basic workaround for not having a game manager - literally just exits the game when the player reaches the south wall trigger
        if (wall.tag == "South")
        {
            body.velocity = Vector3.zero;
            body.GetComponent<Movement>().airControl = false;
            var targetRenderer = target.GetComponent<Renderer>();
            targetRenderer.material.SetColor("_Color", Color.black);
            Debug.Log("Exit");
            Application.Quit();
        }
    }
}
