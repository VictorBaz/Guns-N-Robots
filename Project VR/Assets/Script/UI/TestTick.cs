using System;
using Script.Manager;
using UnityEditor.Rendering;
using UnityEngine;

namespace Script.UI
{
    public class TestTick : MonoBehaviour
    {
        private void OnEnable()
        {
            TickManager.OnTick += DebugTick;
        }

        private void OnDisable()
        {
            TickManager.OnTick -= DebugTick;
        }


        private void DebugTick()
        {
            Debug.Log("TIC");
        }
    }
}