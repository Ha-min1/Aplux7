#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SQLite4Unity3d;
using System.IO;

public class DatabaseInitializer : EditorWindow
{
    [MenuItem("Tools/Initialize Database")]
    public static void InitializeDatabase()
    {
        string dbPath = Path.Combine(Application.dataPath, "StreamingAssets", "PlayerProgress.db");
        string directory = Path.GetDirectoryName(dbPath);
        
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }

        var connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        connection.CreateTable<DatabaseManager.PlayerProgress>();
        connection.Close();

        Debug.Log("Database initialized at: " + dbPath);
        AssetDatabase.Refresh();
    }
}
#endif
