using System.Collections.Generic;
using Script.Enum;
using UnityEngine;

namespace Script.Manager
{
    public class BarelManager
    {
        public void SetupBarrel(List<BarrelHoleState> barel)
        {
            barel.Clear();
            const int numberOfHoles = 6;

            for (int i = 0; i < numberOfHoles; i++)
            {
                barel.Add((BarrelHoleState)Random.Range((int)BarrelHoleState.Empty,(int)BarrelHoleState.Full + 1));
            }
            
            //TODO dont forget to check if all full or all empty
            
            //Done Setup
        }
        
        public int IncrementBarrelByTick(List<BarrelHoleState> barel, int actualIndex)
        {
            int newIndex = actualIndex + 1;
            
            if (newIndex > barel.Count - 1) // then should iterate
            {
                return 0;
            }
            else
            {
                return newIndex;
            }
        }
    }
}