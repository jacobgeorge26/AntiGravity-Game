using System;
using UnityEngine;

[Serializable]
public class PlayerManager
{
    public Color playerColor;
    public Transform spawnPoint;
    [HideInInspector] public int playerNumber;
    [HideInInspector] public string colouredPlayerText;
    [HideInInspector] public GameObject instance;
    [HideInInspector] public int wins;

    private Movement movement;
    private GameObject canvasGameObject;

    //this script exists but hates me so has been left - beware!
    //mostly from Tanks so expect a lot of incorrect variable names and unnecessary lines
    //especially ignore camera stuff - seperate code for that in Movement (and MouseLook)


    public void Setup()
    {
        movement = instance.GetComponent<Movement>();
        canvasGameObject = instance.GetComponentInChildren<Canvas>().gameObject;

        //movement.playerNumber = playerNumber;

        colouredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(playerColor) + ">PLAYER " + playerNumber + "</color>";

        MeshRenderer[] renderers = instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = playerColor;
        }
    }


    public void DisableControl()
    {
        //m_Movement.enabled = false;
        //m_Shooting.enabled = false;

        canvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        //m_Movement.enabled = true;
        //m_Shooting.enabled = true;

        canvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        instance.transform.position = spawnPoint.position;
        instance.transform.rotation = spawnPoint.rotation;

        instance.SetActive(false);
        instance.SetActive(true);
    }
}
