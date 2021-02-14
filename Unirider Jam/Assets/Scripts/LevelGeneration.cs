using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGeneration : MonoBehaviour
{
    public GameObject startingPiece;
    public GameObject player;
    private GameObject currentTrack;
    private GameObject splitReturnTrack;

    public Transform trackStartLocation;
    public Transform playerStartLocation;
    private Transform localEndLocation;

    public int numberOfTracks = 5;
    public int numberOfTracksPerSplit = 3;
    private int currentNumberOfTracks;
    private int currentNumberOfBackTracks;
    private int currentWeight;
    private int playerScore;

    public TrackPiece[] trackPieces;

    private List<GameObject> spawnedTracks = new List<GameObject>();
    private List<GameObject> spawnedSplitTracks = new List<GameObject>();

    public bool escEndGame;
    private bool generateBackwards;
    private bool splitGeneration;

    public Text scoreText;

    void Start()
    {
        StartGeneration();
    }

    void Update()
    {
        // Test Track Generation
        if (Input.GetKeyDown(KeyCode.F))
        {
            GenerateRandomTrack();
        }

        // Restart the game
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        if(escEndGame && Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void StartGeneration()
    {
        // Set player starting position and rotation
        if (player)
        {
            if (playerStartLocation)
            {
                player.transform.position = playerStartLocation.position;
                player.transform.rotation = playerStartLocation.rotation;

                if(player.GetComponent<UnicycleMovement>() && player.GetComponent<UnicycleMovement>().childCycle)
                {
                    player.GetComponent<UnicycleMovement>().childCycle.transform.rotation = playerStartLocation.rotation;

                    player.GetComponent<UnicycleMovement>().startingLocation = playerStartLocation;
                }

                if (player.GetComponent<Rigidbody>())
                {
                    player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                }
            }
        }

        // Place Start Piece
        if (startingPiece && trackStartLocation)
        {
            GameObject spawnedPiece = Instantiate(startingPiece, trackStartLocation.position, trackStartLocation.rotation);
            AddTrack(spawnedPiece);

            GenerateRandomTrack();
        }
        else
        {
            Debug.Log("Missing starting piece or starting location");
        }
    }

    // Generate next track piece
    private void GenerateRandomTrack()
    {
        if (splitReturnTrack)
        {
            GameObject st = Instantiate(splitReturnTrack, localEndLocation.position, localEndLocation.rotation);

            AddTrack(st);

            splitReturnTrack = null;

            // If there are not enough generated tracks, then generate the next track
            if (spawnedTracks.Count < numberOfTracks)
            {
                if (st && st.GetComponent<TrackPiece>() && !st.GetComponent<TrackPiece>().generateBackwards)
                {
                    Invoke("GenerateRandomTrack", 0.1f);
                }
            }
        }
        else
        {
            // Make a list of possible tracks to generate (possible tracks depend on current track weight)
            List<GameObject> possibleTracks = new List<GameObject>();

            // Add the tracks that can be generated
            if (currentWeight <= 1)
            {
                foreach (TrackPiece piece in trackPieces)
                {
                    if (piece.trackWeight > 0 && piece.trackWeight <= 3)
                    {
                        possibleTracks.Add(piece.trackPiece);
                    }
                }
            }
            else if (currentWeight == 2)
            {
                foreach (TrackPiece piece in trackPieces)
                {
                    if (piece.trackWeight >= 0 && piece.trackWeight < 2)
                    {
                        possibleTracks.Add(piece.trackPiece);
                    }
                }
            }
            else if (currentWeight > 2)
            {
                foreach (TrackPiece piece in trackPieces)
                {
                    if (piece.trackWeight <= 1)
                    {
                        possibleTracks.Add(piece.trackPiece);
                    }
                }
            }

            // Generate a random track from the possible tracks
            int randomTrackIndex = Random.Range(0, possibleTracks.Count);
            Transform spawnPoint = spawnedTracks[spawnedTracks.Count - 1].GetComponent<TrackPiece>().endLocation[0];

            GameObject spawnTrack = Instantiate(possibleTracks[randomTrackIndex], spawnPoint.position, spawnPoint.rotation);

            AddTrack(spawnTrack);

            // Check to see if the piece generates backwards
            if (spawnTrack && spawnTrack.GetComponent<TrackPiece>() && spawnTrack.GetComponent<TrackPiece>().generateBackwards)
            {
                generateBackwards = true;
            }

            // Check to see if the piece is a split track
            if (spawnTrack && spawnTrack.GetComponent<TrackPiece>() && spawnTrack.GetComponent<TrackPiece>().splitTrack)
            {
                List<GameObject> posTracks = new List<GameObject>();

                foreach (TrackPiece tp in trackPieces)
                {
                    if (tp.GetComponent<TrackPiece>().isStright)
                    {
                        posTracks.Add(tp.trackPiece);
                    }
                }

                GameObject previousTrack = null;

                foreach (Transform endlocation in spawnTrack.GetComponent<TrackPiece>().endLocation)
                {
                    for (int i = 1; i < numberOfTracksPerSplit; i++)
                    {
                        int rand = Random.Range(0, posTracks.Count);

                        if (i == 1)
                        {
                            previousTrack = Instantiate(posTracks[rand], endlocation.position, endlocation.rotation);
                        }
                        else
                        {
                            previousTrack = Instantiate(posTracks[rand], previousTrack.GetComponent<TrackPiece>().endLocation[0].position, previousTrack.GetComponent<TrackPiece>().endLocation[0].rotation);
                        }

                        spawnedSplitTracks.Add(previousTrack);

                        Debug.Log("Spawn Split track numb " + i + " / " + numberOfTracksPerSplit);
                    }
                }

                if (previousTrack.GetComponent<TrackPiece>().splitTrackEndPiece)
                {
                    splitReturnTrack = previousTrack.GetComponent<TrackPiece>().splitTrackEndPiece;
                }
                else
                {
                    int rand = Random.Range(0, posTracks.Count);

                    splitReturnTrack = posTracks[rand];

                    localEndLocation = splitReturnTrack.GetComponent<TrackPiece>().endLocation[0];
                }
            }

            // If there are not enough generated tracks, then generate the next track
            if (spawnedTracks.Count < numberOfTracks)
            {
                if (spawnTrack && spawnTrack.GetComponent<TrackPiece>() && !spawnTrack.GetComponent<TrackPiece>().generateBackwards)
                {
                    Invoke("GenerateRandomTrack", 0.1f);
                }
            }
        }
    }

    // Add a track to the spawned track list
    private void AddTrack(GameObject track)
    {
        if(spawnedTracks.Count >= numberOfTracks)
        {
            Destroy(spawnedTracks[0], 1);
            spawnedTracks.RemoveAt(0);
        }

        spawnedTracks.Add(track);

        currentWeight = track.GetComponent<TrackPiece>().trackWeight;

        currentNumberOfTracks++;
    }

    // Remove a specific track
    public void RemoveTrack(GameObject track)
    {
        foreach(GameObject sTrack in spawnedTracks)
        {
            if(sTrack == track)
            {
                currentNumberOfTracks--;
            }
        }
    }

    // When the player enters a new track, decide weather to despawn previous tracks & generate new ones
    public void ChangeTrack(GameObject track)
    {
        // Checks if there is already a current track
        if (track.GetComponent<TrackPiece>())
        {
            if (currentTrack)
            {
                // If there is a current track, then despawn previous track and generate new one
                if(track != currentTrack)
                {
                    AddScore(currentTrack.GetComponent<TrackPiece>().trackScore);

                    if (!generateBackwards)
                    GenerateRandomTrack();

                    currentTrack = track;
                }
            }
            else
            {
                currentTrack = track;
            }

            // If the piece can generate backwards, then despawn all previous tracks and generate the next ones.
            if (track.GetComponent<TrackPiece>().generateBackwards)
            {
                generateBackwards = false;

                ClearTracks();

                spawnedTracks.Add(track);
                currentNumberOfTracks = 1;

                track.GetComponent<TrackPiece>().generateBackwards = false;

                Invoke("GenerateRandomTrack", 0.1f);
            }
        }
    }

    // Add score
    public void AddScore(int score)
    {
        playerScore += score;

        if (scoreText)
            scoreText.text = "Score = " + playerScore;
        else
            Debug.Log("missing score text");
    }

    // Restart Game
    public void RestartGame()
    {
        ClearTracks();

        currentNumberOfBackTracks = 0;
        currentNumberOfTracks = 0;
        currentWeight = 0;

        currentTrack = null;
        generateBackwards = false;

        playerScore = 0;
        AddScore(0);

        StartGeneration();
    }

    // Clear track lists
    void ClearTracks()
    {
        List<GameObject> otherTracks = new List<GameObject>();
        otherTracks = spawnedTracks;

        // Despawn all previous tracks
        foreach (GameObject otherTrack in otherTracks)
        {
            Destroy(otherTrack);
        }

        List<GameObject> otherSplitTracks = new List<GameObject>();
        otherSplitTracks = spawnedSplitTracks;

        // Despawn all previous split tracks
        foreach (GameObject otherTrack in otherSplitTracks)
        {
            Destroy(otherTrack);
        }

        spawnedTracks.Clear();
        spawnedSplitTracks.Clear();
    }
}
