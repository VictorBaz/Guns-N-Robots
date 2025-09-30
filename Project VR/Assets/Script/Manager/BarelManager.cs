using System.Collections.Generic;
using System.Linq;
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

            do
            {
                barel.Clear();
                for (int i = 0; i < numberOfHoles; i++)
                {
                    barel.Add((BarrelHoleState)Random.Range((int)BarrelHoleState.Empty, (int)BarrelHoleState.Full + 1));
                }
            }
            while (!HasBothStates(barel));
        }

        private bool HasBothStates(List<BarrelHoleState> barel)
        {
            bool hasEmpty = barel.Contains(BarrelHoleState.Empty);
            bool hasFull = barel.Contains(BarrelHoleState.Full);
            return hasEmpty && hasFull;
        }
        
        public int IncrementBarrelByTick(List<BarrelHoleState> barel, int actualIndex)
        {
            int newIndex = actualIndex + 1;
            
            if (newIndex > barel.Count - 1) 
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