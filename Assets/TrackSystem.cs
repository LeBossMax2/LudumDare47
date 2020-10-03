using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackSystem : MonoBehaviour
{
    public Track straightPrefab;
    public Track anglePrefab;

    private readonly Dictionary<Vector2Int, Track> tracks = new Dictionary<Vector2Int, Track>();

    private void Awake()
    {
        foreach (Track t in GetComponentsInChildren<Track>())
        {
            tracks.Add(Vector2Int.RoundToInt(new Vector2(t.transform.position.x, t.transform.position.z)), t);
        }
    }

    public bool IsRail(Vector2Int pos)
    {
        return tracks.ContainsKey(pos);
    }

    public void UpdatePosition(ref Vector2Int trackpos, ref Vector3 pos, float deltaPos)
    {
        if (!tracks.ContainsKey(trackpos))
        {
            return;
        }

        int i = 0;
        while (deltaPos > 0)
        {
            tracks[trackpos].UpdatePosition(ref trackpos, ref pos, ref deltaPos);
            i++;
            if (i > 10)
                break;
        }
    }

    public void FollowPosition(ref Vector2Int trackpos, ref Vector3 pos, Vector3 target, float targetDist)
    {
        if (tracks.ContainsKey(trackpos))
        {
            int i = 0;
            while ((pos - target).sqrMagnitude > targetDist * targetDist)
            {
                tracks[trackpos].FollowPosition(ref trackpos, ref pos, target, targetDist);
                i++;
                if (i > 10)
                    break;
            }
        }
        else
        {
            var dist = Vector3.Distance(pos, target);
            pos = Vector3.Lerp(target, pos, targetDist / dist);
            trackpos = Vector2Int.RoundToInt(new Vector2(pos.x, pos.z));
        }
    }

    public void AddTracks(List<Vector2Int> points)
    {
        var first = points[0];
        var last = points[points.Count - 1];

        var firstPrev = tracks[first].prev;
        var lastNext = tracks[last].next;

        //Remove old rails
        Vector2Int pos = first;
        while (pos != last)
        {
            Track t = tracks[pos];
            tracks.Remove(pos);
            pos += t.next;
            Destroy(t.gameObject);
        }

        Destroy(tracks[last].gameObject);

        SpawnRail(first, firstPrev, points[1] - first);
        SpawnRail(last, points[points.Count - 2] - last, lastNext);

        // Add new rails
        for (int i = 1; i < points.Count - 1; i++)
        {
            var p = points[i];
            var prev = points[i - 1];
            var next = points[i + 1];

            SpawnRail(p, prev - p, next - p);
        }
    }

    private void SpawnRail(Vector2Int p, Vector2Int prev, Vector2Int next)
    {
        var pos = new Vector3(p.x, 0, p.y);
        var baseRot = Quaternion.LookRotation(new Vector3(next.x, 0, next.y));
        Track t;
        if (prev == next * -1)
        {
            t = Instantiate(straightPrefab, pos, baseRot, this.transform);
        }
        else
        {
            bool reverse = next.y * prev.x - next.x * prev.y > 0;
            if (reverse)
            {
                baseRot *= Quaternion.LookRotation(Vector3.right);
            }
            t = Instantiate(anglePrefab, pos, baseRot, this.transform);

            if (reverse)
            {
                var initialtnext = t.next;
                t.next = t.prev;
                t.prev = initialtnext;
            }
        }
        this.tracks[p] = t;
    }
}
