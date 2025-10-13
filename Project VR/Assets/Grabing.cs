using System;
using Script.Controller;
using Script.Enum;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class Grabing : MonoBehaviour
{
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
    
    void Awake()
    {
        layerMask = LayerMask.GetMask( "Head", "Wall");
    }

    private void Update()
    {
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward)*50, Color.red);
        
        /*if (m_TriggerInput != null)
        {
            var triggerVal = m_TriggerInput.ReadValue();

            if (triggerVal > 0 && canShoot && isGunInHand)
            {
                canShoot = false;
                Shoot();
                Debug.Log("piou");
            }
            
            if (triggerVal < 0.0001f && isGunInHand)
            {
                canShoot = true;
            }
        }*/
        
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

    private void Shoot()
    {
        Debug.Log(playerController.currentCylinderHole);
        if (playerController?.currentCylinderHole == CylinderHoleState.Empty)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, hit.point);
                Debug.Log("hihi");
            }
        }
    }

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

    private void TakeGun(Collider other)
    {
        isGunInHand = true;
        GameObject obj = other.transform.parent.gameObject;
        obj.transform.rotation = objPosInHand.transform.rotation;
        obj.transform.position = objPosInHand.transform.position;
        obj.transform.parent = objPosInHand; 
    }

    private void ThrowGun(Collider other)
    {
        isGunInHand = false;
        GameObject obj = other.transform.parent.gameObject;
        obj.transform.parent = null;
    }
}
