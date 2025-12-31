using System;
using Script.Manager;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Script.UI
{
    public class UiMain : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameObject startGamePanel;
        [SerializeField] private GameObject parametersPanel;
        [SerializeField] private GameObject trainingRoomPanel;
        [SerializeField] private Text scorePlayer;

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

        #region Unity Methods

        private void Awake()
        {
            scorePlayer.text = "";
        }

        #endregion

        #region State Manager

        private void InGame()
        {
            startGamePanel.SetActive(true);
            parametersPanel.SetActive(true);
            trainingRoomPanel.SetActive(true);
            
            if (MiniGameManager.Instance != null)
            {
                scorePlayer.text = $"You killed {MiniGameManager.Instance.ScorePlayer().ToString()} bad guys";
            }
        }

        private void OnGameStart()
        {
            startGamePanel.SetActive(false);
            parametersPanel.SetActive(false);
            trainingRoomPanel.SetActive(false);

        }

        #endregion
    }
}
