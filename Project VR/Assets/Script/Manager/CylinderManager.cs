using System.Collections.Generic;
using System.Linq;
using Script.Enum;
using UnityEngine;

namespace Script.Manager
{
    public class CylinderManager
    {
        public void SetupBarrel(List<CylinderHoleState> barel)
        {
            barel.Clear();
            const int numberOfHoles = 6;

            do
            {
                barel.Clear();
                for (int i = 0; i < numberOfHoles; i++)
                {
                    barel.Add((CylinderHoleState)Random.Range((int)CylinderHoleState.Empty, (int)CylinderHoleState.Full + 1));
                }
            }
            while (!HasBothStates(barel));
        }

        private bool HasBothStates(List<CylinderHoleState> barel)
        {
            bool hasEmpty = barel.Contains(CylinderHoleState.Empty);
            bool hasFull = barel.Contains(CylinderHoleState.Full);
            return hasEmpty && hasFull;
        }
        
        public int IncrementBarrelByTick(List<CylinderHoleState> barel, int actualIndex)
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