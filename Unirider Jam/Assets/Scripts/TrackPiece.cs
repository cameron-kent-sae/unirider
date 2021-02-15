using UnityEngine;

public class TrackPiece : MonoBehaviour
{
    public GameObject trackPiece;
    public GameObject splitTrackEndPiece;
    public GameObject consecutiveTrack;
    public Transform[] endLocation;

    [Range(0, 3)]
    public int trackWeight;
    public int trackScore = 1;

    [HideInInspector] 
    public int trackHealth = 1;

    public bool generateBackwards;
    public bool splitTrack;
    public bool isStickey;
    public bool isStright;

    private void Start()
    {
        if (generateBackwards)
            trackHealth = 2;
    }
}

public class ConsecutiveTrack
{
    private GameObject consecutiveTrack;

    private Transform spawnLocation;

    public bool HasConsecutiveTrack()
    {
        if (consecutiveTrack == null)
        {
            return (false);
        }
        else
        {
            return (true);
        }
    }

    public void SetConsecutiveTrack(GameObject track)
    {
        consecutiveTrack = track;
    }

    public void SetSpawnLocation(Transform spawnPoint)
    {
        spawnLocation = spawnPoint;
    }

    public GameObject GetConsecutiveTrack()
    {
        if (consecutiveTrack != null)
        {
            return (consecutiveTrack);
        }
        else
        {
            return (null);
        }
    }

    public Transform GetSpawnLocation()
    {
        if (spawnLocation != null)
        {
            return (spawnLocation);
        }
        else
        {
            return (null);
        }
    }

    public void ClearConsecutiveTrack()
    {
        consecutiveTrack = null;
        spawnLocation = null;
    }
}
