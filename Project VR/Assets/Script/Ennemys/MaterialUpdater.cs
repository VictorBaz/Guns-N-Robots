using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script.Ennemys
{
    public class MaterialUpdater : MonoBehaviour
    {
        [SerializeField] private List<MeshRenderer> _renderers;
        [SerializeField] private float _duration = 2f;

        private float _progression;

        public void UpdateMaterials()
        {
            StartCoroutine(UpdateMaterialsCoroutine());
        }

        IEnumerator UpdateMaterialsCoroutine()
        {
            while (_progression < 1)
            {
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                foreach (var renderer in _renderers)
                {
                    mpb.SetFloat("_Progression", _progression);
                    renderer.SetPropertyBlock(mpb);
                }
            
                _progression += Time.deltaTime / _duration;
                _progression = Mathf.Clamp01(_progression);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}