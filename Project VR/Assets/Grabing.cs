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
    [SerializeField] private Rigidbody rbParent;
    [SerializeField] private Transform objPosInHand;
    [SerializeField] private GameObject gunObj;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private PlayerController playerController;
    private bool canTakeGun = false;
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
        rbParent.isKinematic = true;
        isGunInHand = true;
        handVisu.enabled = false;
        gunObj.transform.rotation = objPosInHand.transform.rotation;
        gunObj.transform.position = objPosInHand.transform.position;
        gunObj.transform.parent = objPosInHand; 
    }

    private void ThrowGun(Collider other)
    {
        rbParent.isKinematic = false;
        //need to get velocity
        isGunInHand = false;
        handVisu.enabled = true;
        gunObj.transform.parent = null;
    }

    #endregion

    
}
