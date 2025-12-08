using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Controller
{
    public class SizeControllerPlayer : MonoBehaviour
    {
        #region Fields

        [SerializeField] private float baseSize = 1.85f;
        [SerializeField] private Transform cameraOffSettTransform;
        [SerializeField] private Transform parentVrTransform;
        
      
        private float basePlayerHeight;

        [SerializeField] private Slider sliderSize;

        #endregion

        #region Getter Setter

        [SerializeField] private Text textSize;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            InitParameters();
            ApplyNewSize();
        }

        #endregion

        #region Observer

        private void OnEnable()
        {
            sliderSize.onValueChanged.AddListener(delegate { UpdateText();});
        }

        #endregion
        
        #region Init

        private void InitParameters()
        {
            basePlayerHeight = cameraOffSettTransform.position.y;
            
            sliderSize.minValue = 1;
            sliderSize.maxValue = 2;

            sliderSize.value = baseSize;
            
        }

        #endregion

        #region Size Methods

        private float GetNewYHeight()
        {
            float crossProduct = (basePlayerHeight * sliderSize.value) / baseSize;
            float newSize = crossProduct-basePlayerHeight;
            return newSize;
        }

        #endregion

        #region UI helper

        public void ApplyNewSize()
        {
            parentVrTransform.position = new Vector3(parentVrTransform.position.x ,GetNewYHeight(), parentVrTransform.position.z);
        }

        public void ResetSize()
        {
            parentVrTransform.position = new Vector3(parentVrTransform.position.x ,0, parentVrTransform.position.z);
            sliderSize.value = baseSize;
        }

        private void UpdateText()
        {
            textSize.text = $"size : {sliderSize.value * 100} cm";
        }

        #endregion
    }
}
