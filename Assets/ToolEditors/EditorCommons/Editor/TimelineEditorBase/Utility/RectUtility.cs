using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.TimelineEditor
{
    public static class RectUtility
    {
        public static Rect ExpandBy(this Rect rect, float margin)
        {
            return Rect.MinMaxRect(rect.xMin - margin, rect.yMin - margin, rect.xMax + margin, rect.yMax + margin);
        }
        //Get a rect that encapsulates all provided rects
        public static Rect GetBoundRect(params Rect[] rects)
        {
            var xMin = float.PositiveInfinity;
            var xMax = float.NegativeInfinity;
            var yMin = float.PositiveInfinity;
            var yMax = float.NegativeInfinity;

            for (var i = 0; i < rects.Length; i++)
            {
                xMin = Mathf.Min(xMin, rects[i].xMin);
                xMax = Mathf.Max(xMax, rects[i].xMax);
                yMin = Mathf.Min(yMin, rects[i].yMin);
                yMax = Mathf.Max(yMax, rects[i].yMax);
            }

            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        //Get a rect that encapsulates all provided positions
        public static Rect GetBoundRect(params Vector2[] positions)
        {
            var xMin = float.PositiveInfinity;
            var xMax = float.NegativeInfinity;
            var yMin = float.PositiveInfinity;
            var yMax = float.NegativeInfinity;

            for (var i = 0; i < positions.Length; i++)
            {
                xMin = Mathf.Min(xMin, positions[i].x);
                xMax = Mathf.Max(xMax, positions[i].x);
                yMin = Mathf.Min(yMin, positions[i].y);
                yMax = Mathf.Max(yMax, positions[i].y);
            }

            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        public static bool Encapsulates(this Rect a, Rect b)
        {
            if (a == default(Rect) || b == default(Rect)) { return false; }
            return a.x < b.x && a.xMax > b.xMax && a.y < b.y && a.yMax > b.yMax;
        }

    }



}
