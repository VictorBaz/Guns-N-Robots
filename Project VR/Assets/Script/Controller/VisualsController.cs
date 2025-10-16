using System.Collections.Generic;
using UnityEngine;

namespace Script.Controller
{
    public class VisualsController : MonoBehaviour
    {
        private static readonly int ReloadAnim = Animator.StringToHash("Reload");
        private static readonly int ShootAnim = Animator.StringToHash("Shoot");

        #region Fields
        
        [Header("Animator")] [Space(2)]
        [SerializeField] private Animator animatorGun;

        [Header("Particle System")] [Space(2)] 
        
        [SerializeField] private ParticleSystem sparks;

        [SerializeField] private List<ParticleSystem> muzzle;

        [SerializeField] private ParticleSystem bulletShell;

        #endregion

        #region ParticleSystem Methods

        public void Muzzle()
        {
            foreach (var particle in muzzle) TriggerParticleSystem(particle);
        }

        public void Sparks(Vector3 targetPosition)
        {
            sparks.transform.position = targetPosition;
            sparks.Emit(1);
        }

        public void BulletShell() => TriggerParticleSystem(bulletShell);

        private void TriggerParticleSystem(ParticleSystem particleSystem)
        {
            particleSystem.Emit(1);
        }

        #endregion

        #region Animator Methods

        private void AnimTrigger(int id)
        {
            animatorGun.SetTrigger(id);
        }

        public void Reload() => AnimTrigger(ReloadAnim);

        public void Shoot() => AnimTrigger(ShootAnim);


        #endregion
    }
}
