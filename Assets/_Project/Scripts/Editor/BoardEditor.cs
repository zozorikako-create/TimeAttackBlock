#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using TimeAttackBlock.Gameplay;

[CustomEditor(typeof(Board))]
public class BoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var board = (Board)target;

        EditorGUILayout.Space();
        if (GUILayout.Button("Rebuild Grid"))
        {
            board.RebuildGrid();
            EditorUtility.SetDirty(board);
        }

        EditorGUILayout.HelpBox("cellSize ��ς����� Rebuild Grid �������ƌ����ڂ��X�V�ł��܂��B", MessageType.Info);
    }
}
#endif