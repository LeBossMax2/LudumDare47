using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    public float speed = 0;

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
        tracks.UpdatePosition(ref trackpos, ref pos, speed * Time.deltaTime);
        transform.position = pos;
    }
}
