using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    private Vector3 start;
    private Vector3 end;
    public Vector2Int prev;
    public Vector2Int next;

    public void Start()
    {
        // Adjust the fields to the rotation and position of the game object
        Vector3 prev3 = transform.rotation * new Vector3(prev.x, 0, prev.y);
        prev = Vector2Int.RoundToInt(new Vector2(prev3.x, prev3.z));

        Vector3 next3 = transform.rotation * new Vector3(next.x, 0, next.y);
        next = Vector2Int.RoundToInt(new Vector2(next3.x, next3.z));

        start = prev3 * 0.5f + transform.position;
        end = next3 * 0.5f + transform.position;
    }

    public void UpdatePosition(ref Vector2Int trackpos, ref Vector3 pos, ref float deltaPos)
    {
        float dist = Vector3.Distance(pos, this.end);
        if (dist > deltaPos)
        {
            pos = Vector3.Lerp(this.end, this.start, (dist - deltaPos) / Vector3.Distance(this.end, this.start));
            deltaPos = 0;
        }
        else
        {
            pos = this.end;
            trackpos += this.next;
            deltaPos -= dist;
        }
    }

    public void FollowPosition(ref Vector2Int trackpos, ref Vector3 pos, Vector3 target, float targetDist)
    {
        float dist = Vector3.Distance(target, this.end);
        if (dist < targetDist)
        {
            Vector3 axis = this.start - this.end;
            axis.Normalize();

            float tproj = Vector3.Dot(target - this.end, axis);

            Vector3 toTarget = target - this.end - axis * tproj;

            float factor = Mathf.Sqrt(targetDist * targetDist - toTarget.sqrMagnitude) + tproj;

            pos = this.end + axis * factor;
        }
        else
        {
            pos = this.end;
            trackpos += this.next;
        }
    }
}
