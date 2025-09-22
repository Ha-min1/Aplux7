using UnityEngine;
using SQLite4Unity3d; // SQLite4Unity3d 패키지 필요함, 나중에 unity에 넣어놓는거 잊지말기
using System.IO;
using System;

public class DatabaseManager : MonoBehaviour
{
    private SQLiteConnection dbConnection;
    private static DatabaseManager instance;

    // 시간 seed 기반 무작위 playerID 생성, 예: P1234567890 (앞 6자리 무작위, 뒤 4자리 시간) 
    // 후에 직접 입력으로 확장시킬 수도 있음
    // 웹을 통해 계정 연동도 고려 가능
        public static string GenerateRandomPlayerId()
    {
        long seed = DateTime.Now.Ticks;
        System.Random rand = new System.Random((int)(seed & 0xFFFFFFFF));
        string id = "P" + rand.Next(100000, 999999).ToString() + seed.ToString().Substring(seed.ToString().Length - 4);
        return id;
    }

    public static DatabaseManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DatabaseManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("DatabaseManager");
                    instance = obj.AddComponent<DatabaseManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDatabase()
    {
        string dbName = "PlayerProgress.db";
        string dbPath;

#if UNITY_EDITOR
        dbPath = Path.Combine(Application.dataPath, "StreamingAssets", dbName);
#else
        dbPath = Path.Combine(Application.persistentDataPath, dbName);
#endif

        // 디렉토리 생성 확인
        string directory = Path.GetDirectoryName(dbPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 데이터베이스 연결
        dbConnection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Database initialized at: " + dbPath);

        // 테이블 생성
        dbConnection.CreateTable<PlayerProgress>();
    }

    // PlayerProgress 모델 클래스
    public class PlayerProgress
    {
        [PrimaryKey]
        public string player_id { get; set; }
        public float play_time { get; set; }
        public int currency { get; set; }
        public int high_score { get; set; }
        public DateTime last_played { get; set; } // DateTime으로 변경
    }

    // 데이터 저장
    public void SaveProgress(string playerId, float playTime, int currency, int highScore)
    {
        try
        {
            var existingPlayer = dbConnection.Find<PlayerProgress>(playerId);

            if (existingPlayer != null)
            {
                // 기존 데이터 업데이트
                existingPlayer.play_time = playTime;
                existingPlayer.currency = currency;
                existingPlayer.high_score = highScore;
                existingPlayer.last_played = DateTime.Now;
                dbConnection.Update(existingPlayer);
            }
            else
            {
                // 새 데이터 삽입
                var newPlayer = new PlayerProgress
                {
                    player_id = playerId,
                    play_time = playTime,
                    currency = currency,
                    high_score = highScore,
                    last_played = DateTime.Now
                };
                dbConnection.Insert(newPlayer);
            }

            Debug.Log($"Progress saved for player: {playerId}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveProgress error: {ex.Message}");
        }
    }

    // 데이터 로드
    public PlayerProgress LoadProgress(string playerId)
    {
        try
        {
            var player = dbConnection.Find<PlayerProgress>(playerId);
            if (player != null)
            {
                Debug.Log($"Progress loaded for player: {playerId}");
            }
            else
            {
                Debug.Log($"No progress found for player: {playerId}");
            }
            return player;
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadProgress error: {ex.Message}");
            return null;
        }
    }

    // 모든 플레이어 데이터 조회 (디버깅용)
    public System.Collections.Generic.List<PlayerProgress> GetAllPlayers()
    {
        try
        {
            return dbConnection.Table<PlayerProgress>().ToList();
        }
        catch (Exception ex)
        {
            Debug.LogError($"GetAllPlayers error: {ex.Message}");
            return new System.Collections.Generic.List<PlayerProgress>();
        }
    }

    // 앱 종료 시 DB 연결 해제
    void OnApplicationQuit()
    {
        if (dbConnection != null)
        {
            dbConnection.Close();
            Debug.Log("Database connection closed");
        }
    }

    void OnDestroy()
    {
        if (dbConnection != null && instance == this)
        {
            dbConnection.Close();
        }
    }
}
