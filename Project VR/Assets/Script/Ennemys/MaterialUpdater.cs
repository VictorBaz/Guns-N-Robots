using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script.Ennemys
{
    public class MaterialUpdater : MonoBehaviour
    {
        #region Fields

        [SerializeField] private List<MeshRenderer> _renderers;
        [SerializeField] private float _duration = 2f;

        private float _progression;

        #endregion
        
        //MERCI ANTONIN LE GOAT 
        #region Update Materials Methods

        public void UpdateMaterials()
        {
            StartCoroutine(UpdateMaterialsCoroutine());
        }

        IEnumerator UpdateMaterialsCoroutine()
        {
            while (_progression < 1)
            {
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                foreach (var _mat in _renderers)
                {
                    mpb.SetFloat("_Progression", _progression);
                    _mat.SetPropertyBlock(mpb);
                }
            
                _progression += Time.deltaTime / _duration;
                _progression = Mathf.Clamp01(_progression);
                yield return new WaitForSeconds(0.05f);
            }
        }

        public void ResetMaterial()
        {
            StopAllCoroutines();
            _progression = 0f;

            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetFloat("_Progression", _progression);

            foreach (var renderer in _renderers)
            {
                renderer.SetPropertyBlock(mpb);
            }
        }


        #endregion
        
    }
}