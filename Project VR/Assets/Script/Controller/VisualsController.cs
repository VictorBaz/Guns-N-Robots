using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script.Controller
{
    public class VisualsController : MonoBehaviour
    {
        #region Fields
        
        private static readonly int ReloadAnim = Animator.StringToHash("Reload");
        private static readonly int ShootAnim = Animator.StringToHash("Shoot");
        
        [Header("Animator")] [Space(2)]
        [SerializeField] private Animator animatorGun;

        [Header("Particle System")] [Space(2)] 
        
        [SerializeField] private ParticleSystem sparks;

        [SerializeField] private List<ParticleSystem> muzzle;

        [SerializeField] private List<ParticleSystem> bulletShell;

        [SerializeField] private List<AnimationClip> allAnimation;

        #endregion

        #region ParticleSystem Methods

        public void Muzzle()
        {
            foreach (var particle in muzzle) TriggerParticleSystem(particle);
        }

        public void Sparks(Vector3 targetPosition, Vector3 normalHit)
        {
            sparks.transform.rotation = Quaternion.LookRotation(normalHit);
            sparks.transform.position = targetPosition;
            sparks.Play();
        }

        public void BulletShellEffect(float time) => StartCoroutine(BulletShellEffectIE(time));
        
        IEnumerator BulletShellEffectIE(float time)
        {
            foreach (var shell in bulletShell)
            {
                shell.Clear();
                shell.Stop();
            }
            
            float _time = time / 6;
            
            foreach (var shell in bulletShell)
            {
                shell.Play();
                yield return new WaitForSeconds(_time);
            }
        }

        private void TriggerParticleSystem(ParticleSystem _particleSystem)
        {
            _particleSystem.Clear();
            _particleSystem.Play();
        }

        #endregion

        #region Animator Methods

        private float AnimTrigger(int id,int index)
        {
            animatorGun.SetTrigger(id);
            return allAnimation[index].length;
        }

        public float Reload() => AnimTrigger(ReloadAnim,1);

        public void Shoot() => AnimTrigger(ShootAnim,2);


        #endregion
    }
}
