using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool TestMode = false;
    public static UnityEvent OnGameFailed = new UnityEvent();

    private static GameManager Instance;
    private bool GameEnded;
    private bool bIsGamePaused = false;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
        DisableCursor();
    }

    private void Start()
    {
        if (TestMode)
        {
            ResourceManager.CollectableGained(1000); // add 1000 resources
        }
    }

    private void Update()
    {
        if (TestMode) TestModeUpdate();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void TestModeUpdate()
    {
        if (Input.GetKey(KeyCode.R))
        {
            GameEnded = true;
            ReloadScene();
        }
    }

    public static void GoalReached()
    {
        if (Instance.GameEnded)
        {
            return;
        }

        var CurrentScene = SceneManager.GetActiveScene();
        ResourceManager.Commit();
        if (CurrentScene.buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            // game ended
            print("The game has ended.");
            SceneManager.LoadScene(0);
        }
        else
        {
            // go to the next scene
            SceneManager.LoadScene(CurrentScene.buildIndex + 1);
        }
    }

    public static void GameFailed()
    {
        if (Instance.GameEnded)
        {
            return;
        }

        Instance.GameEnded = true;
        OnGameFailed.Invoke();
        ResourceManager.Revert();
        Instance.StartCoroutine(Instance.GameEndedDelay());
    }

    private IEnumerator GameEndedDelay()
    {
        yield return new WaitForSeconds(2f);
        ReloadScene();
    }

    private static void ReloadScene()
    {
        var CurrentScene = SceneManager.GetActiveScene();
        Instance.GameEnded = false;
        SceneManager.LoadScene(CurrentScene.buildIndex);
    }

    public static void SlowTime()
    {
        Time.timeScale = .2f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

    public static void RestoreTime()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

    public static void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public static void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    public static void PauseGame()
    {
        Instance.bIsGamePaused = true;
    }

    public static void ResumeGame()
    {
        Instance.bIsGamePaused = false;
    }

    public static bool IsGamePaused()
    {
        return Instance.bIsGamePaused;
    }

    public static bool PlayerCanUseAbilities()
    {
        return FindObjectOfType<PlayerController>().CanUseAbility;
    }
}