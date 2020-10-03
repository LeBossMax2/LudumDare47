using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainPart : MonoBehaviour
{
    public GameObject follow;
    public float dist;

    private Vector2Int trackpos;
    private TrackSystem tracks;

    private void Awake()
    {
        tracks = FindObjectOfType<TrackSystem>();
        trackpos = Vector2Int.RoundToInt(new Vector2(transform.position.x, transform.position.z));
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        tracks.FollowPosition(ref trackpos, ref pos, this.follow.transform.position, this.dist);
        transform.position = pos;
    }
}
