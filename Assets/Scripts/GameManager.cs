using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   
   [SerializeField] private bool TestMode = false;
   
   private static GameManager Instance;

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
         ReloadScene();
      }
   }

   public static void GoalReached()
   {
      var CurrentScene = SceneManager.GetActiveScene();
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
      ReloadScene();
   }

   private static void ReloadScene()
   {
      var CurrentScene = SceneManager.GetActiveScene();
      SceneManager.LoadScene(CurrentScene.buildIndex);
   }
}