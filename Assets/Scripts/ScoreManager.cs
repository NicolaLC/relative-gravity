using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
   private static ScoreManager Instance;

   private int GoldGained = 0;
   private int SilverGained = 0;
   private int BronzeGained = 0;
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
   }

   public static void CollectableGained(ECollectableType collectableType)
   {
      switch (collectableType)
      {
         case ECollectableType.Gold:
            Instance.GoldGained++;
            break;
         case ECollectableType.Silver:
            Instance.SilverGained++;
            break;
         case ECollectableType.Bronze:
            Instance.BronzeGained++;
            break;
      }
      
   }
}
