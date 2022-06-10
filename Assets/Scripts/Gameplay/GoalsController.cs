using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoalsController : MonoBehaviour {
    
    public List<GoalData> activeGoals = new List<GoalData>();

    public class GoalsRefreshEvent : UnityEvent { }
    [HideInInspector] public GoalsRefreshEvent onGoalsRefresh = new GoalsRefreshEvent();

    public class GoalsCompleteEvent : UnityEvent { }
    [HideInInspector] public GoalsCompleteEvent onGoalsComplete = new GoalsCompleteEvent();



    public void Init(List<GoalData> newGoals) {
        activeGoals = new List<GoalData>();
        foreach(var goal in newGoals)
            activeGoals.Add(new GoalData(goal.gType, goal.value, goal.icon));
    }


    public void Refresh() {
        foreach(var goal in activeGoals) {
            switch(goal.gType) {

                case GoalType.CollectGreen:
                    goal.value = Mathf.Clamp(goal.value - Player.Instance.qBitsCollected[QBitType.GREEN], 0, goal.value);
                    Player.Instance.qBitsCollected[QBitType.GREEN] = 0;
                    break;
                case GoalType.CollectBlue:
                    goal.value = Mathf.Clamp(goal.value - Player.Instance.qBitsCollected[QBitType.BLUE], 0, goal.value);
                    Player.Instance.qBitsCollected[QBitType.BLUE] = 0;
                    break;
                case GoalType.CollectRed:
                    goal.value = Mathf.Clamp(goal.value - Player.Instance.qBitsCollected[QBitType.RED], 0, goal.value);
                    Player.Instance.qBitsCollected[QBitType.RED] = 0;
                    break;

                case GoalType.KillWorms:
                    goal.value = Mathf.Clamp(goal.value - Player.Instance.enemiesKilled[EnemyType.Worm], 0, goal.value);
                    Player.Instance.enemiesKilled[EnemyType.Worm] = 0;
                    break;
                case GoalType.KillSkeletons:
                    goal.value = Mathf.Clamp(goal.value - Player.Instance.enemiesKilled[EnemyType.Skeleton], 0, goal.value);
                    Player.Instance.enemiesKilled[EnemyType.Skeleton] = 0;
                    break;
                case GoalType.KillZombies:
                    goal.value = Mathf.Clamp(goal.value - Player.Instance.enemiesKilled[EnemyType.Zombie], 0, goal.value);
                    Player.Instance.enemiesKilled[EnemyType.Zombie] = 0;
                    break;
                case GoalType.KillAgents:
                    goal.value = Mathf.Clamp(goal.value - Player.Instance.enemiesKilled[EnemyType.Agent], 0, goal.value);
                    Player.Instance.enemiesKilled[EnemyType.Agent] = 0;
                    break;
                case GoalType.KillSpiders:
                    goal.value = Mathf.Clamp(goal.value - Player.Instance.enemiesKilled[EnemyType.Spider], 0, goal.value);
                    Player.Instance.enemiesKilled[EnemyType.Spider] = 0;
                    break;

                case GoalType.DestroySkeletonSpawners:
                    goal.value = Mathf.Clamp(goal.value - Player.Instance.skeletonSpawnersDestroyed, 0, goal.value);
                    Player.Instance.skeletonSpawnersDestroyed = 0;
                    break;
            }
        }

        onGoalsRefresh.Invoke();

        if(IsComplete())
            onGoalsComplete.Invoke();
    }


    public bool IsComplete() {
        foreach(var goal in activeGoals) {
            if(goal.value > 0)
                return false;
        }

        return true;
    }
}
