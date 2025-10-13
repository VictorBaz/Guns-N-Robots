using System.Collections.Generic;
using System.Linq;
using Script.Enum;
using UnityEngine;

namespace Script.Manager
{
    public class CylinderManager
    {

        #region Setup

        public void SetupBarrelRandom(List<CylinderHoleState> barel)
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
        
        public void SetupBarrel(List<CylinderHoleState> barel)
        {
            barel.Clear();
            const int numberOfHoles = 6;
            for (int i = 0; i < numberOfHoles; i++) barel.Add(CylinderHoleState.Full);    
        }

        public void Reload(List<CylinderHoleState> barel) => SetupBarrel(barel);

        #endregion

        #region Check

        private bool HasBothStates(List<CylinderHoleState> barel)
        {
            bool hasEmpty = barel.Contains(CylinderHoleState.Empty);
            bool hasFull = barel.Contains(CylinderHoleState.Full);
            return hasEmpty && hasFull;
        }
        
        
        #endregion

        #region Cylinder Methods Linked to Tick

        public int IncrementBarrelByTick(List<CylinderHoleState> barel, int actualIndex)
        {
            return (actualIndex + 1) % barel.Count;
        }

        #endregion
        
    }
}