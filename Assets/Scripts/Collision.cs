using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Collision : MonoBehaviour
{
    public Rigidbody body; //player
    public GameObject target; //cursor target

    private Movement movement; //movement script

    //when the player is 'stuck' to a wall they can move
    //when they're in the air they're stuck on that course of motion

    public void OnTriggerEnter(Collider coll)
    {
        //if the player is what hit the collider - otherwise it's just something boring
        if (coll.gameObject.tag == "Player")
        {
            //get movement script to allow changes to the movement cos we like to move it move it
            movement = body.GetComponent<Movement>();
            //change target colour to tell user whether they can move or not
            var targetRenderer = target.GetComponent<Renderer>();
            targetRenderer.material.SetColor("_Color", Color.red);
            //allow air control so they can move on respawn - necessary for if a laser hit them in midair
            movement.airControl = true;
            //stop their velocity or it'll be really annoying
            body.velocity = Vector3.zero;
            body.useGravity = false;
            //reset bool hit (by laser) as the player is...not hit by a laser anymore...cos they've been reset...
            movement.hit = false;
        }


        //this code is intended to cause lasers to bounce off walls/obstacles but currently doesn't work
        //problem with using trigger is it doesn't allow for contactPoints which means no normals
        //probably needs to figure out primary direction of the laser
        //then multiply the velocity in that direction by -1

        /*
        else if (coll.gameObject.tag == "Laser")
        {
            Rigidbody laser = coll.GetComponent<Rigidbody>();
            Transform motion = laser.transform;
            //Vector3 velocity = motion.forward;
            Vector3 velocity = laser.velocity;

            float[] values = { velocity.x, velocity.y, velocity.z };
            float max = values.Select(System.Math.Abs).Max();
            int maxIndex = values.ToList().IndexOf(max);
            if (maxIndex == -1)
                maxIndex = values.ToList().IndexOf(max * -1);
            values[maxIndex] *= -1;

            Vector3 newVelocity = new Vector3();
            newVelocity.x = values[0];
            newVelocity.y = values[1];
            newVelocity.z = values[2];

            laser.velocity = newVelocity;
            //Vector3 collisionPoint = coll.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            //Vector3 normal = collisionPoint.normal;
            //Vector3 newVelocity = Vector3.Reflect(velocity, coll.contacts[0].normal);
            //laser.velocity = newVelocity;
        }
        */

    }

    public void OnTriggerExit(Collider coll)
        //lock the player's ability to choose a new path of motion until they collide with a wall again
        //could do with a failsafe of if they've been out of contact with a wall for x seconds they're unlocked again
    {
        if (coll.gameObject.tag == "Player" && !movement.hit)
        {
            var targetRenderer = target.GetComponent<Renderer>();
            targetRenderer.material.SetColor("_Color", Color.black);
            movement.airControl = false;
        }
    }



}
