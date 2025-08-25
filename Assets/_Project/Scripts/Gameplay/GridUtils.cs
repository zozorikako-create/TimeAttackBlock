using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TimeAttackBlock
{
    public static class GridUtils
    {
        // Rotate 90 degrees: (x,y) -> (-y, x)
        public static Vector2Int Rot90(Vector2Int p) => new Vector2Int(-p.y, p.x);

        // Apply k times 90Åã rotation and then normalize to start at (0,0)
        public static List<Vector2Int> RotateAndNormalize(List<Vector2Int> cells, int k)
        {
            var cs = new List<Vector2Int>(cells);
            k = ((k % 4) + 4) % 4;
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < cs.Count; j++)
                {
                    cs[j] = Rot90(cs[j]);
                }
            }

            int minX = cs.Min(c => c.x);
            int minY = cs.Min(c => c.y);
            for (int i = 0; i < cs.Count; i++)
            {
                cs[i] = new Vector2Int(cs[i].x - minX, cs[i].y - minY);
            }
            return cs;
        }
    }
}