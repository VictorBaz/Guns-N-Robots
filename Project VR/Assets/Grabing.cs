using System;
using Script.Controller;
using Script.Enum;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class Grabing : MonoBehaviour
{
    #region Fields

    [SerializeField] private MeshRenderer handVisu;
    [SerializeField] private Transform objPosInHand;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private PlayerController playerController;
    private bool canTakeGun = false;
    private bool canShoot = true;
    public bool isGunInHand = false;
    [SerializeField]
    XRInputValueReader<float> m_TriggerInput = new XRInputValueReader<float>("Trigger");
    [SerializeField]
    XRInputValueReader<float> m_GripInput = new XRInputValueReader<float>("Grip");
    private RaycastHit hit;

    private Collider currentGun;
    LayerMask layerMask;

    #endregion

    #region Unity Methods

    void Awake()
    {
        layerMask = LayerMask.GetMask( "Head", "Wall");
    }

    private void Update()
    {
        
        if (m_GripInput != null)
        {
            var gripVal = m_GripInput.ReadValue();
            if (gripVal > 0 && canTakeGun)
            {
                canTakeGun = false;
                TakeGun(currentGun);
            }

            if (gripVal < 0.0001f && isGunInHand)
            {
                canTakeGun = true;
                ThrowGun(currentGun);
            }
        }
    }

    #endregion

    #region Physic Methods

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gun"))
        {
            canTakeGun = true;
            currentGun = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gun"))
        {
            canTakeGun = false;
        }
    }

    #endregion

    #region Grabbing Methods

    private void TakeGun(Collider other)
    {
        isGunInHand = true;
        handVisu.enabled = false;
        GameObject obj = other.transform.parent.gameObject;
        obj.transform.rotation = objPosInHand.transform.rotation;
        obj.transform.position = objPosInHand.transform.position;
        obj.transform.parent = objPosInHand; 
    }

    private void ThrowGun(Collider other)
    {
        isGunInHand = false;
        handVisu.enabled = true;
        GameObject obj = other.transform.parent.gameObject;
        obj.transform.parent = null;
    }

    #endregion

    
}
