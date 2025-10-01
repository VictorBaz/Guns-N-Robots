using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class Grabing : MonoBehaviour
{
    [SerializeField] private Transform objPosInHand;
    private bool canTakeGun = false;
    private bool canShoot = true;
    public bool isGunInHand = false;
    [SerializeField]
    XRInputValueReader<float> m_TriggerInput = new XRInputValueReader<float>("Trigger");
    [SerializeField]
    XRInputValueReader<float> m_GripInput = new XRInputValueReader<float>("Grip");

    private Collider currentGun;
    private void Update()
    {
        if (m_TriggerInput != null)
        {
            var triggerVal = m_TriggerInput.ReadValue();

            if (triggerVal > 0 && canShoot && isGunInHand)
            {
                canShoot = false;
                Debug.Log("piou");
            }
            
            if (triggerVal < 0.0001f && isGunInHand)
            {
                canShoot = true;
            }
        }
        
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
