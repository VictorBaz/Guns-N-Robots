using Script.Interface;
using Script.Manager;
using UnityEngine;
using UnityEngine.Events;

public class ShootingInteractUnityEvent : MonoBehaviour,IDamagable
{
    [SerializeField] private UnityEvent eventToDo;
    
    public void TakeDamage()
    {
        eventToDo?.Invoke();
    }

    

    public bool CanTakeDamage { get; set; } = true;
}
