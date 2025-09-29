using System.Collections.Generic;
using Script.Enum;
using UnityEngine;

namespace Script.Manager
{
    public class BarelManager : MonoBehaviour
    {
        private void SetupBarrel(List<BarrelHoleState> barel)
        {
            barel.Clear();
            const int numberOfHoles = 5;

            for (int i = 0; i < numberOfHoles; i++)
            {
                barel.Add((BarrelHoleState)Random.Range((int)BarrelHoleState.Empty,(int)BarrelHoleState.Full + 1));
            }
            //Done Setup
        }
    }
}