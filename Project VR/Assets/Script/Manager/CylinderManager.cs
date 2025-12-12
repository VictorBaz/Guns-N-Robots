using System.Collections.Generic;
using System.Linq;
using Script.Enum;
using UnityEngine;

namespace Script.Manager
{
    public class CylinderManager
    {

        #region Setup And Reload
        
        public void SetupBarrel(List<CylinderHoleState> barel)
        {
            barel.Clear();
            const int numberOfHoles = 6;
            for (int i = 0; i < numberOfHoles; i++) barel.Add(CylinderHoleState.Full);    
        }

        public void Reload(List<CylinderHoleState> barel) => SetupBarrel(barel);

        #endregion

        #region Cylinder Methods Linked to Tick

        public int IncrementBarrelByTick(List<CylinderHoleState> barel, int actualIndex)
        {
            return (actualIndex + 1) % barel.Count;
        }

        #endregion
        
    }
}