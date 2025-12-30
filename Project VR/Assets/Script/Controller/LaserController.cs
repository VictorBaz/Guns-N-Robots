using System;
using Script.Manager;
using UnityEngine;

namespace Script.Controller
{
    public class LaserController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private float maxLength = 25f;

        [Header("Laser Origin & End")]
        [SerializeField] private Transform startPos;
        private Vector3 endPos;

        private Ray ray;
        private RaycastHit hitInfo;

        private LineRenderer effect;

        private bool activeLaser;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            effect = GetComponent<LineRenderer>();
            OnGameEnd();
        }

        private void Update()
        {
            UpdateLaser();
        }

        private void Start()
        {
            OnGameStart();
        }

        #endregion

        /*
        #region Observer

        private void OnEnable()
        {
            EventManager.OnGameStart += OnGameStart;
            EventManager.OnGameEnd += OnGameEnd;
        }

        private void OnDisable()
        {
            EventManager.OnGameStart -= OnGameStart;
            EventManager.OnGameEnd -= OnGameEnd;
        }

        #endregion*/

        #region State Handle

        private void OnGameStart()
        {
            activeLaser = true;
            effect.enabled = true;
        }

        private void OnGameEnd()
        {
            activeLaser = false;
            effect.enabled = false;
        }

        #endregion

        #region Draw Laser

        private void UpdateLaser()
        {
            if (!activeLaser) return;
            
            ray.origin = startPos.position;
            ray.direction = startPos.forward;

            if (Physics.Raycast(ray, out hitInfo, maxLength))
            {
                endPos = hitInfo.point;
            }
            else
            {
                endPos = ray.origin + ray.direction * maxLength;
            }

            effect.SetPosition(0, startPos.position);
            effect.SetPosition(1, endPos);
        }

        #endregion
        
    }
}