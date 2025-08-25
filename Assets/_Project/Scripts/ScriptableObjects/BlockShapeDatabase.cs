using System;
using System.Collections.Generic;
using UnityEngine;

namespace TimeAttackBlock
{
    [CreateAssetMenu(menuName = "TimeAttackBlock/Block Shape Database")]
    public class BlockShapeDatabase : ScriptableObject
    {
        [Serializable]
        public class BlockShape
        {
            [Tooltip("Unique ID like P01, P02 ...")]
            public string id;

            [Tooltip("Human readable name")]
            public string displayName;

            [Tooltip("Base cells with origin at (0,0) (unrotated).")]
            public List<Vector2Int> cells = new List<Vector2Int>();
        }

        [Header("List of 13 shapes (SPEC_FULL2)")]
        public List<BlockShape> shapes = new List<BlockShape>();

#if UNITY_EDITOR
        [ContextMenu("Fill Default 13 Shapes (SPEC_FULL2)")]
        private void FillDefaults_SPEC_FULL2()
        {
            shapes = new List<BlockShape>
            {
                // P01 L triomino A (3): (0,0),(1,0),(1,1)
                new BlockShape{ id="P01", displayName="L Triomino A (3)",
                    cells=new List<Vector2Int>{ new(0,0), new(1,0), new(1,1) } },

                // P02 L triomino B (3): (0,0),(0,1),(1,1)
                new BlockShape{ id="P02", displayName="L Triomino B (3)",
                    cells=new List<Vector2Int>{ new(0,0), new(0,1), new(1,1) } },

                // P03 L triomino C (3): (1,0),(0,1),(1,1)
                new BlockShape{ id="P03", displayName="L Triomino C (3)",
                    cells=new List<Vector2Int>{ new(1,0), new(0,1), new(1,1) } },

                // P04 3x3 square (9)
                new BlockShape{ id="P04", displayName="3x3 Square (9)",
                    cells=new List<Vector2Int>{
                        new(0,0),new(1,0),new(2,0),
                        new(0,1),new(1,1),new(2,1),
                        new(0,2),new(1,2),new(2,2)
                    } },

                // P05 T tetromino (4): (0,0),(1,0),(2,0),(1,1)
                new BlockShape{ id="P05", displayName="T Tetromino (4)",
                    cells=new List<Vector2Int>{ new(0,0), new(1,0), new(2,0), new(1,1) } },

                // P06 2x2 square (4)
                new BlockShape{ id="P06", displayName="2x2 Square (4)",
                    cells=new List<Vector2Int>{
                        new(0,0), new(1,0),
                        new(0,1), new(1,1)
                    } },

                // P07 L tetromino R (4): (0,0),(0,1),(0,2),(1,2)
                new BlockShape{ id="P07", displayName="L Tetromino R (4)",
                    cells=new List<Vector2Int>{ new(0,0), new(0,1), new(0,2), new(1,2) } },

                // P08 L tetromino L (4): (1,0),(1,1),(1,2),(0,2)
                new BlockShape{ id="P08", displayName="L Tetromino L (4)",
                    cells=new List<Vector2Int>{ new(1,0), new(1,1), new(1,2), new(0,2) } },

                // P09 L pentomino (5): (0,0),(0,1),(0,2),(1,2),(2,2)
                new BlockShape{ id="P09", displayName="L Pentomino (5)",
                    cells=new List<Vector2Int>{ new(0,0), new(0,1), new(0,2), new(1,2), new(2,2) } },

                // P10 I pentomino (5): (0,0),(1,0),(2,0),(3,0),(4,0)
                new BlockShape{ id="P10", displayName="I Pentomino (5)",
                    cells=new List<Vector2Int>{ new(0,0), new(1,0), new(2,0), new(3,0), new(4,0) } },

                // P11 I tetromino (4): (0,0),(1,0),(2,0),(3,0)
                new BlockShape{ id="P11", displayName="I Tetromino (4)",
                    cells=new List<Vector2Int>{ new(0,0), new(1,0), new(2,0), new(3,0) } },

                // P12 I triomino (3): (0,0),(1,0),(2,0)
                new BlockShape{ id="P12", displayName="I Triomino (3)",
                    cells=new List<Vector2Int>{ new(0,0), new(1,0), new(2,0) } },

                // P13 Single (1): (0,0)
                new BlockShape{ id="P13", displayName="Single (1)",
                    cells=new List<Vector2Int>{ new(0,0) } },
            };
            Debug.Log("[BlockShapeDatabase] Filled SPEC_FULL2 default 13 shapes.");
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}