using System;
using TMPro;
using UnityEngine;

namespace Script.Interact
{
    public class SignLoadingScene : ShootingInteractUnityEvent
    {
        [field: SerializeField, TextArea] private string text;
        [field: SerializeField] private TMP_Text textField;

        private void Start()
        {
            textField.text = text;
        }
    }
}