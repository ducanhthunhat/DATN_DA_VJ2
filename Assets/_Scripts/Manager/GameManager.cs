using System;
using System.Collections.Generic;
using UnityEngine;
using DucAnh;

public class GameManager : Singleton<GameManager>
{
    public event Action<GameState> OnGameStateChanged;

    private GameState currentGameState = GameState.UI;

    private const string SAVE_LEVEL_KEY = "SavedLevel";

    [Header("Level Spawning")]
    [SerializeField] private List<GameObject> levelPrefabs = new List<GameObject>();
    private GameObject currentLevelInstance;
    public GameObject CurrentLevelInstance => currentLevelInstance;
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
        // Xóa save cũ để đảm bảo chơi lại từ đầu
        PlayerPrefs.DeleteKey(SAVE_LEVEL_KEY);
        PlayerPrefs.Save();

        currentLevelIndex = 0;
        LoadLevel(currentLevelIndex);
        ChangeState(GameState.Gameplay);
    }

    public void RestartGame()
    {
        Debug.Log("[GameManager] Người chơi đã chết. Bắt đầu lại từ Level 1...");
        StartGame(); // Gọi hàm StartGame để xóa Save và quay về Level đầu
        
        // Hiện màn hình Loading
        UIManager.Instance.OpenUI<LoadLVPanel>();
        UIManager.Instance.CloseUI<LoadLVPanel>(3f);
    }

    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey(SAVE_LEVEL_KEY);
    }

    public void ContinueGame()
    {
        if (HasSaveData())
        {
            currentLevelIndex = PlayerPrefs.GetInt(SAVE_LEVEL_KEY);
            LoadLevel(currentLevelIndex);
            ChangeState(GameState.Gameplay);
        }
        else
        {
            Debug.LogWarning("[GameManager] Không có dữ liệu lưu (Save Data) để Continue!");
        }
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        
        // Lưu lại checkpoint khi sang level mới
        PlayerPrefs.SetInt(SAVE_LEVEL_KEY, currentLevelIndex);
        PlayerPrefs.Save();
        
        // Nếu đã chơi hết các level thì quay lại level đầu tiên (hoặc hiện bảng Win tùy bạn)
        if (currentLevelIndex >= levelPrefabs.Count)
        {
            currentLevelIndex = 0; 
        }

        LoadLevel(currentLevelIndex);
    }

    private void LoadLevel(int index)
    {
        // Ẩn thanh máu Boss nếu nó đang bật (đề phòng trường hợp chết khi đang đánh Boss)
        if (BossHealthBar.Instance != null)
        {
            BossHealthBar.Instance.HideBossHealthBar();
        }

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
            // TÌM BẰNG COMPONENT ĐỂ LẤY ĐƯỢC CẢ OBJECT ĐANG BỊ ẨN (INACTIVE) VÌ CHẾT
            Player playerScript = FindObjectOfType<Player>(true);
            if (playerScript != null)
            {
                // Bật lại Player (hồi sinh)
                playerScript.gameObject.SetActive(true);
                playerScript.transform.position = spawnPoint.transform.position;

                // Reset lại máu
                var stats = playerScript.GetComponentInChildren<DucAnh.CoreSystem.Stats>(true);
                if (stats != null)
                {
                    stats.Health.Init(); // Hồi đầy máu
                }

                // Reset vũ khí về trạng thái ban đầu
                var weaponInventory = playerScript.GetComponentInChildren<DucAnh.CoreSystem.WeaponInventory>(true);
                if (weaponInventory != null)
                {
                    weaponInventory.ResetToDefault();
                }

                Debug.Log("[GameManager] Đã hồi sinh và dịch chuyển Player tới điểm xuất phát.");
            }
            else
            {
                Debug.LogWarning("[GameManager] Không tìm thấy GameObject nào chứa script 'Player' để dịch chuyển!");
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