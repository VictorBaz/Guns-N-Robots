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
            const int numberOfHoles = 6; // franchement, j'pense ca devrait être totalement retiré le 6 en dur et mis
                                         // dans uns criptable ou un script de constantes utilisé un peu partout
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