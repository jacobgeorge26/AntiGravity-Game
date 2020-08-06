using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public int numRoundsToWin = 1;
    public float startDelay = 3f;
    public float endDelay = 3f;
    public Text messageText;
    public GameObject body;
    public PlayerManager[] players;
    public bool endGame = false;


    private int roundNumber;
    private WaitForSeconds startWait;
    private WaitForSeconds endWait;
    private PlayerManager roundWinner;
    private PlayerManager gameWinner;


    //this script exists but hates me so has been left - beware!
    //mostly from Tanks so expect a lot of incorrect variable names and unnecessary lines
    //especially ignore camera stuff - seperate code for that in Movement (and MouseLook)
    private void Start()
    {
        startWait = new WaitForSeconds(startDelay);
        endWait = new WaitForSeconds(endDelay);

        SpawnAllPlayers();
        SetCameraTargets();

        StartCoroutine(GameLoop());
    }


    private void SpawnAllPlayers()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].instance =
                Instantiate(body, players[i].spawnPoint.position, players[i].spawnPoint.rotation) as GameObject;
            players[i].playerNumber = i + 1;
            players[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[players.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = players[i].instance.transform;
        }

        //m_CameraControl.m_Targets = targets;
    }


        private IEnumerator GameLoop()
        {
            yield return StartCoroutine(RoundStarting());
            yield return StartCoroutine(RoundPlaying());
            yield return StartCoroutine(RoundEnding());

            if (gameWinner != null)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                StartCoroutine(GameLoop());
            }
        }


        private IEnumerator RoundStarting()
        {
            ResetAllPlayers();
            DisablePlayerControl();

            //m_CameraControl.SetStartPositionAndSize();

            roundNumber++;
            messageText.text = "ROUND " + roundNumber;
            yield return startWait;
        }


        private IEnumerator RoundPlaying()
        {
            EnablePlayerControl();

            messageText.text = string.Empty;

            while (!OnePlayerLeft())
            while(!endGame)
            {
                yield return null;
            }
        }


        private IEnumerator RoundEnding()
        {
            DisablePlayerControl();

            roundWinner = null;

            roundWinner = GetRoundWinner();

            if (roundWinner != null)
                roundWinner.wins++;

            gameWinner = GetGameWinner();

           // string message = EndMessage();
            string message = "Game Over!";
            messageText.text = message;
            yield return endWait;
        }


        private bool OnePlayerLeft()
        {
            int numPlayersLeft = 0;

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].instance.activeSelf)
                    numPlayersLeft++;
            }

            return numPlayersLeft <= 1;
        }


        private PlayerManager GetRoundWinner()
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].instance.activeSelf)
                    return players[i];
            }

            return null;
        }


        private PlayerManager GetGameWinner()
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].wins == numRoundsToWin)
                    return players[i];
            }

            return null;
        }


        private string EndMessage()
        {
            string message = "DRAW!";

            if (roundWinner != null)
                message = roundWinner.colouredPlayerText + " WINS THE ROUND!";

            message += "\n\n\n\n";

            for (int i = 0; i < players.Length; i++)
            {
                message += players[i].colouredPlayerText + ": " + players[i].wins + " WINS\n";
            }

            if (gameWinner != null)
                message = gameWinner.colouredPlayerText + " WINS THE GAME!";

            return message;
        }


        private void ResetAllPlayers()
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].Reset();
            }
        }


        private void EnablePlayerControl()
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].EnableControl();
            }
        }


        private void DisablePlayerControl()
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].DisableControl();
            }
        }

}