using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPiece : MonoBehaviour
{
    public GameObject trackPiece;
    public Transform endLocation;

    [Range(0, 3)]
    public int trackWeight;

    public bool generateBackwards;
    public bool isStickey;
}
