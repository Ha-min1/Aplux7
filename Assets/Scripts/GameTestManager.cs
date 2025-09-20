using UnityEngine;

public class GameTestManager : MonoBehaviour
{
    void Start()
    {
        TestDatabaseOperations();
    }

    void TestDatabaseOperations()
    {
        // 데이터 저장 테스트
        DatabaseManager.Instance.SaveProgress("test_player_1", 3600.5f, 1500, 1200);
        DatabaseManager.Instance.SaveProgress("test_player_2", 1800.2f, 800, 950);
        
        // 데이터 로드 테스트
        var player1 = DatabaseManager.Instance.LoadProgress("test_player_1");
        var player2 = DatabaseManager.Instance.LoadProgress("test_player_2");
        
        if (player1 != null)
        {
            Debug.Log($"Player1 - Currency: {player1.currency}, High Score: {player1.high_score}");
        }
        
        if (player2 != null)
        {
            Debug.Log($"Player2 - Currency: {player2.currency}, High Score: {player2.high_score}");
        }
        
        // 모든 데이터 조회 (디버깅용)
        var allPlayers = DatabaseManager.Instance.GetAllPlayers();
        Debug.Log($"Total players in database: {allPlayers.Count}");
    }
}
