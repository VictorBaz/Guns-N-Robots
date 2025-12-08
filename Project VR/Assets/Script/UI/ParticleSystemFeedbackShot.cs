using System;
using System.Collections.Generic;
using Script.Enum;
using Script.Manager;
using UnityEngine;

namespace Script.UI
{
    public class ParticleSystemFeedbackShot : MonoBehaviour
    {
        [Header("Particle Systems")]
        [SerializeField] private ParticleSystem badShotParticle;
        [SerializeField] private ParticleSystem goodShotParticle;
        [SerializeField] private ParticleSystem perfectShotParticle;
        [SerializeField] private ParticleSystem perfectBonusParticle;
        [SerializeField] private ParticleSystem missedShotParticle;

        private List<ParticleSystem> allEffect = new List<ParticleSystem>();

        private void OnEnable()
        {
            EventManager.OnBadShoot += DisplayFeedBack;
            EventManager.OnGoodShot += DisplayFeedBack;
            EventManager.OnPerfectShot += DisplayFeedBack;
            EventManager.OnMissShot += DisplayFeedBack;
        }
        
        private void OnDisable()
        {
            EventManager.OnBadShoot -= DisplayFeedBack;
            EventManager.OnGoodShot -= DisplayFeedBack;
            EventManager.OnPerfectShot -= DisplayFeedBack;
            EventManager.OnMissShot -= DisplayFeedBack;
        }

        private void Awake()
        {
            allEffect.Add(badShotParticle);
            allEffect.Add(goodShotParticle);
            allEffect.Add(perfectShotParticle);
            allEffect.Add(perfectBonusParticle);
            allEffect.Add(missedShotParticle);
        }

        private void DisplayFeedBack(ShotDone shotState)
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.MiniGameRunning)
            {
                return;
            }
            
            switch (shotState)
            {
                case ShotDone.Bad:
                    PlayParticle(badShotParticle);
                    break;
                    
                case ShotDone.Good:
                    PlayParticle(goodShotParticle);
                    break;
                    
                case ShotDone.Perfect:
                    PlayParticle(perfectShotParticle);
                    break;

                case ShotDone.Miss:
                    PlayParticle(missedShotParticle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shotState), shotState, null);
            }
        }

        private void PlayParticle(ParticleSystem particle)
        {
            foreach (var ps in allEffect) ps.Stop();
            
            if (particle != null) particle.Play();
        }
    }
}