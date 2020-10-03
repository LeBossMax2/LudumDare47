using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RailBuilder : MonoBehaviour
{
    public Camera cam;
    public LineRenderer selectionLine;
    public Plane ground = new Plane(Vector3.up, Vector3.up * 0.5f);
    private TrackSystem tracks;
    
    private List<Vector2Int> points = new List<Vector2Int>();

    private void Awake()
    {
        tracks = FindObjectOfType<TrackSystem>();
        if (selectionLine == null)
            selectionLine = GetComponentInChildren<LineRenderer>();
        if (cam == null)
            cam = Camera.main;
    }

    private void Start()
    {
        selectionLine.enabled = false;
    }

    private Vector2Int raycast(Vector3 screenPos)
    {
        float dist;
        var ray = cam.ScreenPointToRay(screenPos);
        ground.Raycast(ray, out dist);

        var point = ray.GetPoint(dist);
        return Vector2Int.RoundToInt(new Vector2(point.x, point.z));
    }

    private void addTracks(Vector2Int targetPos)
    {
        var i = 0;
        while (targetPos != points[points.Count - 1])
        {
            var diff = targetPos - points[points.Count - 1];
            var nextPos = points[points.Count - 1] + (Mathf.Abs(diff.x) > Mathf.Abs(diff.y) ? diff.x > 0 ? Vector2Int.right : Vector2Int.left : diff.y > 0 ? Vector2Int.up : Vector2Int.down);
            if (points.Count > 1 && points[points.Count - 2] == nextPos)
            {
                points.RemoveAt(points.Count - 1);
                selectionLine.positionCount--;
            }
            else
            {
                if (points.Count == 1 && tracks.IsRail(nextPos))
                {
                    points.Clear();
                    points.Add(nextPos);
                    selectionLine.SetPosition(0, new Vector3(nextPos.x, 0.6f, nextPos.y));
                }
                else if ((tracks.IsRail(points[points.Count - 1]) && points.Count > 1) || points.Contains(nextPos) || nextPos == points[0])
                {
                    break;
                }
                else
                {
                    points.Add(nextPos);
                    selectionLine.positionCount++;
                    selectionLine.SetPosition(selectionLine.positionCount - 1, new Vector3(nextPos.x, 0.6f, nextPos.y));
                }
            }

            i++;
            if (i > 10)
                break; // Prevent potential infinite loops
        }
    }

    public void OnBeginDrag(BaseEventData e)
    {
        var ev = (PointerEventData)e;
        
        var trackpos = this.raycast(ev.position);

        if (tracks.IsRail(trackpos))
        {
            points.Clear();
            points.Add(trackpos);
            selectionLine.enabled = true;
            selectionLine.positionCount = 1;
            selectionLine.SetPosition(0, new Vector3(trackpos.x, 0.6f, trackpos.y));
            e.Use();
        }
    }

    public void OnDrag(BaseEventData e)
    {
        var ev = (PointerEventData)e;
        if (points.Count > 0)
        {
            var trackPos = this.raycast(ev.position);
            addTracks(trackPos);
            e.Use();
        }
    }

    public void OnEndDrag(BaseEventData e)
    {
        var ev = (PointerEventData)e;

        if (points.Count > 0)
        {
            addTracks(this.raycast(ev.position));

            if (points.Count > 1 && tracks.IsRail(points[points.Count - 1]))
            {
                tracks.AddTracks(points);
            }

            selectionLine.positionCount = 0;
            selectionLine.enabled = false;
            points.Clear();

            e.Use();
        }
    }
}