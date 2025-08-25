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

        EditorGUILayout.HelpBox("cellSize ‚ğ•Ï‚¦‚½‚ç Rebuild Grid ‚ğ‰Ÿ‚·‚ÆŒ©‚½–Ú‚ğXV‚Å‚«‚Ü‚·B", MessageType.Info);
    }
}
#endif