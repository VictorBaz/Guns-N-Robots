using System;
using Script.Interface;
using UnityEngine;

/// <summary>
/// How dummy works : 2 animations one IDLE and one Dead and Reset mix
/// public void will be used in the animation cant
/// </summary>
public class DummyController : MonoBehaviour, IDamagable
{
    #region Fields

    [SerializeField] private Animator dummyAnimator;
    public bool CanTakeDamage { get; set; } = true;

    #endregion

    public void TakeDamage()
    {
        if (!CanTakeDamage) return;
        Die();
    }

    
    #region State Handler

    private void Die()
    {
        CanTakeDamage = false;
        dummyAnimator.SetTrigger("Death");
    }

    public void Reset()
    {
        CanTakeDamage = true;
        dummyAnimator.Play("Idle");
    }
    

    #endregion
}
