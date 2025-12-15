using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace Script.Controller
{
    public class Grabbing : MonoBehaviour
    {
        #region Fields

        // et ici handMeshRenderer
        [SerializeField] private MeshRenderer handVisu;
        [SerializeField] private Rigidbody rbParent;
        // ici j'aurais surement nommé ca "handTransform"
        [SerializeField] private Transform objPosInHand;
        // attention au naming obj, ca peut laisser penser que c'est une ref vers un fichier .obj, ou un type object, 
        // dans ton cas, j'aurais surement utilisé juste "gun" ou gunGo"
        [SerializeField] private GameObject gunObj;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private PlayerController playerController;
        
        private bool canTakeGun = false;
        private bool isGunInHand = false;
        private RaycastHit hit;
        private Collider currentGun;
        private LayerMask layerMask;
        
        // des fois tu mets mVariable, des fois variable, fais attention à ta nomenclature
        [SerializeField] XRInputValueReader<float> mTriggerInput = new XRInputValueReader<float>("Trigger");
        [SerializeField] XRInputValueReader<float> mGripInput = new XRInputValueReader<float>("Grip");
        
        #endregion

        #region Unity Methods

        void Awake()
        {
            layerMask = LayerMask.GetMask( "Head", "Wall");
        }

        // on évite au maximum le code dans l'update, c'est pas claire ce que ca
        // fait, tu devrais n'avoir que des appels de fonction
        private void Update()
        {
        
            if (mGripInput != null)
            {
                var gripVal = mGripInput.ReadValue();
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
            // -> pas claire ce que ce commentaire apporte
            //need to get velocity
            isGunInHand = false;
            handVisu.enabled = true;
            gunObj.transform.parent = null;
        }

        // tu peux éviter la méthode en faisant un get set comme ca:
       // public bool isGunInHand { public get; private set }
        
        public bool IsGunInHand() => isGunInHand;

        #endregion

    }
}
