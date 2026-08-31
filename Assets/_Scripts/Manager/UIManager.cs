using System.Collections;
using System.Collections.Generic;
using DucAnh;
using UnityEditor;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private List<UICanvas> uiCanvases;
    public Transform _effects;
    private bool isPaused = false;
    private StartPanel startPanel;

    public override void Awake()
    {
        base.Awake();
        // Cực kỳ quan trọng: Nếu đây là UIManager thừa (bị Singleton loại bỏ), dừng chạy code ngay lập tức
        if (Instance != this) return;

        InitializeUICanvases();
        Instance.OpenUI<StartPanel>();
    }

    private void InitializeUICanvases()
    {
        foreach (var canvas in uiCanvases)
        {
            if (canvas == null) continue;

            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            }
            
            canvas.gameObject.SetActive(true); 
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public T OpenUI<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.Setup();
            canvas.Open();
        }
        else
        {
            Debug.LogError($"[UIManager trên GameObject: {gameObject.name}] LỖI: Không tìm thấy UI tên là {typeof(T).Name}. Danh sách uiCanvases hiện có {uiCanvases.Count} phần tử.");
            foreach (var c in uiCanvases)
            {
                if (c != null)
                {
                    Debug.Log($"Phần tử trong danh sách: {c.gameObject.name} (Kiểu script: {c.GetType().Name})");
                }
            }
        }
        return canvas;
    }

    public void CloseUI<T>(float time) where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.Close(time);
        }
    }

    public void CloseUIDirectly<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.CloseDirectly();
        }
    }

    public bool IsUIOpened<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas == null) return false;
        
        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        return canvasGroup != null && canvasGroup.alpha > 0f;
    }

    public T GetUI<T>() where T : UICanvas
    {
        return uiCanvases.Find(c => c is T) as T;
    }
    
    /// <summary>
    /// Opens the gameplay UIs (UIGameplay, UICore) after a specified delay.
    /// </summary>
    /// <param name="delay">Time to wait in seconds before opening the UIs.</param>
    

    
    public void CloseAll()
    {
        foreach (var canvas in uiCanvases)
        {
            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup != null && canvasGroup.alpha > 0f)
            {
                canvas.Close(0);
            }
        }
    }

    private void Update()
    {
        // Lắng nghe phím ESC bằng New Input System
        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Không cho bấm ESC bậy bạ nếu đang Loading hoặc Game Over
            if (IsUIOpened<LoadLVPanel>() || IsUIOpened<GameOverPanel>())
                return;

            // Nếu màn hình Cài đặt (Options) đang bật -> Bấm ESC để đóng nó lại
            if (IsUIOpened<OptionsPanel>())
            {
                CloseUIDirectly<OptionsPanel>();
                return;
            }

            // Nếu màn hình Pause đang bật -> Bấm ESC để đóng và chơi tiếp
            if (IsUIOpened<PausePanel>())
            {
                CloseUIDirectly<PausePanel>();
                ResumeGame();
                return;
            }

            // Nếu đang ở sảnh (StartPanel), đã tắt hết Options, bấm ESC thì không làm gì
            if (IsUIOpened<StartPanel>())
                return;

            // Nếu không có Menu nào đang bật (đang trong lúc chơi) -> Bấm ESC để bật Pause Game
            PauseGame(); 
            OpenUI<PausePanel>();
        }
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}