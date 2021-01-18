using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    public GameObject startingPiece;

    public Transform startLocation;

    public int numberOfTracks = 5;
    private int currentNumberOfTracks;
    private int currentWeight;

    public TrackPiece[] trackPieces;

    private List<GameObject> spawnedTracks = new List<GameObject>();

    private bool generateBackwards;

    void Start()
    {
        StartGeneration();
    }

    void Update()
    {
        
    }

    public void StartGeneration()
    {
        if(startingPiece && startLocation)
        {
            GameObject spawnedPiece = Instantiate(startingPiece, startLocation.position, startLocation.rotation);
            spawnedTracks.Add(spawnedPiece);

            currentNumberOfTracks++;

            GenerateRandomTrack();
        }
        else
        {
            Debug.Log("Missing starting piece or starting location");
        }
    }

    private void GenerateRandomTrack()
    {
        if(currentNumberOfTracks < numberOfTracks)
        {
            List<GameObject> possibleTracks = new List<GameObject>();

            if(currentWeight <= 1)
            {
                foreach(TrackPiece piece in trackPieces)
                {
                    if(piece.trackWeight > 0 && piece.trackWeight <= 3)
                    {
                        possibleTracks.Add(piece.trackPiece);
                    }
                }
            }
            else if(currentWeight == 2)
            {
                foreach (TrackPiece piece in trackPieces)
                {
                    if (piece.trackWeight >= 0 && piece.trackWeight < 2)
                    {
                        possibleTracks.Add(piece.trackPiece);
                    }
                }
            }
            else if(currentWeight > 2)
            {
                foreach (TrackPiece piece in trackPieces)
                {
                    if (piece.trackWeight <= 1)
                    {
                        possibleTracks.Add(piece.trackPiece);
                    }
                }
            }

            int randomTrackIndex = Random.Range(0, possibleTracks.Count);
            Transform spawnPoint = spawnedTracks[spawnedTracks.Count - 1].GetComponent<TrackPiece>().endLocation;

            GameObject spawnTrack = Instantiate(possibleTracks[randomTrackIndex], spawnPoint.position, spawnPoint.rotation);

            AddTrack(spawnTrack);

            if(spawnTrack && spawnTrack.GetComponent<TrackPiece>() && !spawnTrack.GetComponent<TrackPiece>().generateBackwards)
            {
                Invoke("GenerateRandomTrack", 0.1f);
            }
        }
    }

    private void AddTrack(GameObject track)
    {
        //spawnedTracks.RemoveAt(0);

        spawnedTracks.Add(track);

        currentWeight = track.GetComponent<TrackPiece>().trackWeight;

        currentNumberOfTracks++;
    }

    public void RemoveTrack(GameObject track)
    {
        foreach(GameObject sTrack in spawnedTracks)
        {
            if(sTrack == track)
            {
                //spawnedTracks.Remove(track);

                currentNumberOfTracks--;
            }
        }
        //GenerateRandomTrack();
    }

    public void ChangeTrack(GameObject track)
    {
        if(track.GetComponent<TrackPiece>() && track.GetComponent<TrackPiece>().generateBackwards)
        {
            generateBackwards = true;

            List<GameObject> otherTracks = new List<GameObject>();
            otherTracks = spawnedTracks;

            foreach (GameObject otherTrack in otherTracks)
            {
                if (!otherTrack.GetComponent<TrackPiece>().generateBackwards)
                {
                    Destroy(otherTrack);

                    //RemoveTrack(otherTrack);
                }
            }

            spawnedTracks.Clear();

            spawnedTracks.Add(track);
            currentNumberOfTracks = 1;

            track.GetComponent<TrackPiece>().generateBackwards = false;

            Invoke("GenerateRandomTrack", 0.1f);
        }
    }
}
