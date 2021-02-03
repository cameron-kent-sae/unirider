using UnityEngine;

public class TrackPiece : MonoBehaviour
{
    public GameObject trackPiece;
    public Transform endLocation;

    [Range(0, 3)]
    public int trackWeight;
    public int trackScore = 1;

    public bool generateBackwards;
    public bool isStickey;
}
