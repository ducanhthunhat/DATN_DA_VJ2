using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action<GameState> OnGameStateChanged;

    private GameState currentGameState = GameState.UI;

    [Header("Level Spawning")]
    [SerializeField] private List<GameObject> levelPrefabs = new List<GameObject>();
    private GameObject currentLevelInstance;
    private int currentLevelIndex = 0;

    public void ChangeState(GameState state)
    {
        if (state == currentGameState)
            return;

        switch (state)
        {
            case GameState.UI:
                EnterUIState();
                break;
            case GameState.Gameplay:
                EnterGameplayState();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        currentGameState = state;
        OnGameStateChanged?.Invoke(currentGameState);
    }

    public void StartGame()
    {
        currentLevelIndex = 0;
        LoadLevel(currentLevelIndex);
        ChangeState(GameState.Gameplay);
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        
        // Nếu đã chơi hết các level thì quay lại level đầu tiên (hoặc hiện bảng Win tùy bạn)
        if (currentLevelIndex >= levelPrefabs.Count)
        {
            currentLevelIndex = 0; 
        }

        LoadLevel(currentLevelIndex);
    }

    private void LoadLevel(int index)
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        if (index >= 0 && index < levelPrefabs.Count && levelPrefabs[index] != null)
        {
            currentLevelInstance = Instantiate(levelPrefabs[index], Vector3.zero, Quaternion.identity);
            Debug.Log($"[GameManager] Đã load Level {index + 1}");

            // Dịch chuyển Player đến điểm bắt đầu của map mới
            TeleportPlayerToSpawnPoint();
        }
        else
        {
            Debug.LogWarning($"[GameManager] Không tìm thấy Level Prefab ở vị trí số {index}!");
        }
    }

    private void TeleportPlayerToSpawnPoint()
    {
        if (currentLevelInstance == null) return;

        // Tìm điểm spawn trong Level vừa đẻ ra
        PlayerSpawnPoint spawnPoint = currentLevelInstance.GetComponentInChildren<PlayerSpawnPoint>();
        if (spawnPoint != null)
        {
            // Tìm Player trên Scene (thông qua Tag)
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = spawnPoint.transform.position;
                Debug.Log("[GameManager] Đã dịch chuyển Player tới điểm xuất phát của Level.");
            }
            else
            {
                Debug.LogWarning("[GameManager] Không tìm thấy GameObject nào có Tag là 'Player' để dịch chuyển!");
            }
        }
        else
        {
            Debug.LogWarning("[GameManager] Level này không có điểm xuất phát (chưa gắn script PlayerSpawnPoint). Player sẽ đứng im tại chỗ cũ.");
        }
    }

    private void EnterUIState()
    {
        Time.timeScale = 0f;
    }

    private void EnterGameplayState()
    {
        Time.timeScale = 1f;
    }


    public enum GameState
    {
        UI,
        Gameplay
    }
}