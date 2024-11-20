using System;
using UnityEngine;
using KayosStudios.AsteroidQuest.GameManagement;

[DefaultExecutionOrder(-10)]
public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the EventManager between scene loads
        }
        else
            Destroy(gameObject);
    }

    //Event to handle orb interaction
    public event Action OnOrbSelected;
    public void OrbSelected() => OnOrbSelected?.Invoke();

    //Event for when the player completes an objective
    public event Action<ObjectiveTypes> OnObjectiveCompleted;
    public void ObjectiveCompleted(ObjectiveTypes completedObj) => OnObjectiveCompleted?.Invoke(completedObj);

    //Event for when the game starts
    public event Action OnGameStart;
    public void GameStart() => OnGameStart?.Invoke();
}
