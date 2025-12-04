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

        private List<ParticleSystem> allEffect = new List<ParticleSystem>();

        private void OnEnable()
        {
            EventManager.OnBadShoot += DisplayFeedBack;
            EventManager.OnGoodShot += DisplayFeedBack;
            EventManager.OnPerfectShot += DisplayFeedBack;
        }
        
        private void OnDisable()
        {
            EventManager.OnBadShoot -= DisplayFeedBack;
            EventManager.OnGoodShot -= DisplayFeedBack;
            EventManager.OnPerfectShot -= DisplayFeedBack;
        }

        private void Awake()
        {
            allEffect.Add(badShotParticle);
            allEffect.Add(goodShotParticle);
            allEffect.Add(perfectShotParticle);
            allEffect.Add(perfectBonusParticle);
        }

        private void DisplayFeedBack(ShotDone shotState)
        {
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
                  //  PlayParticle(perfectBonusParticle);
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