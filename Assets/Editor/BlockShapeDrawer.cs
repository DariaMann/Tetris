using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlockShape), false)]
[CanEditMultipleObjects]
[System.Serializable]
public class BlockShapeDrawer : Editor
{
    private BlockShape BlockShapeInstance => target as BlockShape;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ClearBoardButton();
        EditorGUILayout.Space();
        
        DrawColumnsInputFields();
        EditorGUILayout.Space();

        if (BlockShapeInstance.board != null && BlockShapeInstance.columns > 0 && BlockShapeInstance.rows > 0)
        {
            DrawBoardTable();
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(BlockShapeInstance);
        }
    }

    private void ClearBoardButton()
    {
        if (GUILayout.Button("Clear Board"))
        {
            BlockShapeInstance.Clear();
        }
    }
    
    private void DrawColumnsInputFields()
    {
        var columnsTemp = BlockShapeInstance.columns;
        var rowsTemp = BlockShapeInstance.rows;

        BlockShapeInstance.columns = EditorGUILayout.IntField("Columns", BlockShapeInstance.columns);
        BlockShapeInstance.rows = EditorGUILayout.IntField("Rows", BlockShapeInstance.rows);

        if ((BlockShapeInstance.columns != columnsTemp || BlockShapeInstance.rows != rowsTemp) &&
            BlockShapeInstance.columns > 0 && BlockShapeInstance.rows > 0)
        {
            BlockShapeInstance.CreateNewBoard();
        }
    }
    
    private void DrawBoardTable()
    {
        var tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);
        tableStyle.margin.left = 32;
        
        var headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 65;
        headerColumnStyle.alignment = TextAnchor.MiddleCenter;
        
        var rowStyle = new GUIStyle();
        rowStyle.fixedWidth = 25;
        rowStyle.alignment = TextAnchor.MiddleCenter;
        
        var dataFielsStyle = new GUIStyle(EditorStyles.miniButtonMid);
        dataFielsStyle.normal.background = Texture2D.grayTexture;
        dataFielsStyle.onNormal.background = Texture2D.whiteTexture;

        for (var row  = 0; row < BlockShapeInstance.rows; row++)
        {
            EditorGUILayout.BeginHorizontal(headerColumnStyle);
            for (var column  = 0; column < BlockShapeInstance.columns; column++)
            {
                EditorGUILayout.BeginHorizontal(rowStyle);
                var data = EditorGUILayout.Toggle(BlockShapeInstance.board[row].column[column], dataFielsStyle);
                BlockShapeInstance.board[row].column[column] = data;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
