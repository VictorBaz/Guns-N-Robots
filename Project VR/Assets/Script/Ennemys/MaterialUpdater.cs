using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialUpdater : MonoBehaviour
{
    private List<Material> _materials;
    private List<MeshRenderer> _renderers;

    private float _progression;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _materials = GetComponentsInChildren<MeshRenderer>().Select((mr)=> mr.material).ToList();
        _renderers = GetComponentsInChildren<MeshRenderer>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMaterials()
    {
        StartCoroutine(UpdateMaterialsCoroutine());
    }

    IEnumerator UpdateMaterialsCoroutine()
    {
        while (_progression < 1)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            foreach (var mat in _renderers)
            {
                mpb.SetFloat("_Progression", _progression);
                mat.SetPropertyBlock(mpb);
            }
            
            float duration = 2;
            _progression += Time.deltaTime / duration;
            _progression = Mathf.Clamp01(_progression);
            yield return new WaitForSeconds(0.05f);
        }
        
    }
}
