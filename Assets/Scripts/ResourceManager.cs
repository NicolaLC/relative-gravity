using Models;
using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour
{
   public static readonly UnityEvent OnResourceUpdate = new UnityEvent();
   private static ResourceManager Instance;

   private Score CurrentScore = new Score();
   private Score CommitedScore = new Score();

   private void Awake()
   {
      if (!Instance)
      {
         Instance = this;
      }
      else
      {
         Destroy(this);
         return;
      }
      DontDestroyOnLoad(this);
   }

   public static void CollectableGained(int Amount)
   {
      Instance.CurrentScore.ResourceCount += Amount;
      while (Instance.CurrentScore.ResourceCount >= 100)
      {
         Instance.CurrentScore.ResourceStack++;
         Instance.CurrentScore.ResourceCount -= 100;
      }
      OnResourceUpdate.Invoke();
   }

   public static int GetCurrentResources()
   {
      return Instance.CurrentScore.ResourceCount;
   }
   public static int GetCurrentStack()
   {
      return Instance.CurrentScore.ResourceStack;
   }

   public static void Commit()
   {
      Instance.CommitedScore = new Score(Instance.CurrentScore);
      OnResourceUpdate.Invoke();
   }

   public static void Revert()
   {
      Instance.CurrentScore = new Score(Instance.CommitedScore);
      OnResourceUpdate.Invoke();
   }

   public static void ConsumeStack()
   {
      Instance.CurrentScore.ResourceStack--;
      OnResourceUpdate.Invoke();
   }
}
