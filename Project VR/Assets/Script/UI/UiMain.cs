using System;
using Script.Manager;
using Unity.VisualScripting;
using UnityEngine;

namespace Script.UI
{
    public class UiMain : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameObject startGamePanel;

        #endregion

        #region Observer

        private void OnEnable()
        {
            EventManager.OnGameStart += OnGameStart;
            EventManager.OnGameEnd += InGame;
        }

        private void OnDisable()
        {
            EventManager.OnGameStart -= OnGameStart;
            EventManager.OnGameEnd -= InGame;
        }

        #endregion

        #region State Manager

        private void InGame()
        {
            startGamePanel.SetActive(true);
        }

        private void OnGameStart()
        {
            startGamePanel.SetActive(false);
        }

        #endregion
    }
}
