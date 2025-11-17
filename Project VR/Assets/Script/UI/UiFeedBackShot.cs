using System;
using System.Collections;
using Script.Enum;
using Script.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Script.UI
{
    public class UiFeedBackShot : MonoBehaviour
    {
        [SerializeField] private Sprite[] spriteFeedBackShot;
        [SerializeField] private Image spriteOfFeedBack;

        private Coroutine _displayShotFeedBackCoroutine;


        private void Awake()
        {
            spriteOfFeedBack.sprite = null;
        }

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



        private void DisplayFeedBack(ShotDone shotState)
        {
            if (_displayShotFeedBackCoroutine != null)
            {
                StopCoroutine(_displayShotFeedBackCoroutine);
                spriteOfFeedBack.sprite = null;
            }
            
            switch (shotState)
            {
                case ShotDone.Bad:
                    _displayShotFeedBackCoroutine = StartCoroutine(DisplayFeedBackIe(0));
                    break;
                case ShotDone.Good:
                    _displayShotFeedBackCoroutine = StartCoroutine(DisplayFeedBackIe(1));
                    break;
                case ShotDone.Perfect:
                    _displayShotFeedBackCoroutine = StartCoroutine(DisplayFeedBackIe(2));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shotState), shotState, null);
            }
        }

        private IEnumerator DisplayFeedBackIe(int index)
        {
            spriteOfFeedBack.sprite = spriteFeedBackShot[index];
            Color color = spriteOfFeedBack.color;
            color.a = 1f;
            spriteOfFeedBack.color = color;

            float duration = TickManager.TimeBetweenTick;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Lerp(1f, 0f, elapsed / duration);
                spriteOfFeedBack.color = color;
                yield return null;
            }

            spriteOfFeedBack.sprite = null;
        }

    }
}
