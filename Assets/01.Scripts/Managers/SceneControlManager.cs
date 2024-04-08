using System;
using ManagingSystem;
using UnityEngine;

public class SceneControlManager : BaseManager<SceneControlManager>
{
    [SerializeField] private SceneType _startSceneType;
    
    public Scene CurrentScene { get; private set; }
    public PlayerUnit Player => CurrentScene == null ? null : CurrentScene.Player;

    [SerializeField] private float _fadeTime;
    [SerializeField] private float _loadingTime;

    private SceneTransitionCanvas _currentCanvas;

    public override void StartManager()
    {
        LoadScene(_startSceneType);
    }

    public void LoadScene(SceneType type, Action onSceneCreate = null, Action onLoadedCallback = null, bool loading = true)
    {
        if (_currentCanvas != null) return;

        _currentCanvas = PoolManager.Instance.Pop("SceneTransitionCanvas") as SceneTransitionCanvas;
        _currentCanvas.PresentTransition(0.0f, 1.0f, _fadeTime, () =>
        {
            if (CurrentScene is not null)
            {
                PoolManager.Instance.Push(CurrentScene);
            }

            CurrentScene = PoolManager.Instance.Pop($"{type}Scene") as Scene;
            onSceneCreate?.Invoke();
            CurrentScene.onLoadScene?.Invoke();
            
            CurrentScene.CreatePlayer();

            //위에 함수가 전부다 정상 작동 했을 경우 밑에 있는 것을 실행시켜주어야 함
            _currentCanvas.PauseTransition(_loadingTime, () =>
            {
                _currentCanvas.PresentTransition(1.0f, 0.0f, _fadeTime, () =>
                {
                    onLoadedCallback?.Invoke();
                    PoolManager.Instance.Push(_currentCanvas);
                    _currentCanvas = null;
                });
            });
        });
    }

    public PoolableMono AddObject(string id)
    {
        if (CurrentScene is null)
        {
            Debug.LogError("[SceneControlManager] currentScene is null. returning null");
            return null;
        }
        return CurrentScene.AddObject(id);
    }

    public void DeleteObject(PoolableMono obj)
    {
        if (CurrentScene is null)
        {
            Debug.LogError("[SceneControlManager] currentScene is null.");
            return;
        }
        CurrentScene.DeleteObject(obj);
    }

    public void SafeDeleteObject(PoolableMono obj)
    {
        if (CurrentScene is null)
        {
            Debug.LogError("[SceneControlManager] currentScene is null.");
            return;
        }
        CurrentScene.SafeDeleteObject(obj);
    }
}