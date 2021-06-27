using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportObject : MonoBehaviour
{
    public float moveSpeed;
    public GameObject teleportObject;
    private GameObject copyObject;
    private Material material;
    private Material copyMaterial;

    void Start()
    {
        material = teleportObject.GetComponent<MeshRenderer>().material;
        material.SetInt("toggle", 0);
        copyObject = Instantiate(teleportObject);
        copyObject.SetActive(false);
        copyObject.GetComponent<TeleportObject>().enabled = false;
        copyObject.GetComponent<Collider>().enabled = false;
        copyMaterial = copyObject.GetComponent<MeshRenderer>().material;
        copyMaterial.SetInt("toggle", 0);
    }

    private void Update()
    {
        var w = Input.GetAxis("Mouse ScrollWheel");
        transform.position = transform.position + transform.forward* w * moveSpeed * Time.smoothDeltaTime;
    }
    private bool clipDir;
    void OnTriggerEnter(Collider other)
    {
        Portal portal = other.gameObject.GetComponent<Portal>() as Portal;
        if (portal != null)
        {
            material.SetInt("toggle", 1);
            copyMaterial.SetInt("toggle", 1);
            portal = other.gameObject.GetComponent<Portal>();
            copyObject.SetActive(true);

            var tr = portal.transform.InverseTransformPoint(teleportObject.transform.position);
            if (tr.z <= 0f)
            {
                clipDir = true;
            }
            else
            {
                clipDir = false;
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        Portal portal = other.gameObject.GetComponent<Portal>() as Portal;
        if (portal != null)
            Clipping(portal);
    }
    void OnTriggerExit(Collider other)
    {
        Portal portal = other.gameObject.GetComponent<Portal>() as Portal;
        if (portal != null) {
            var tr = portal.transform.InverseTransformPoint(teleportObject.transform.position);
            if ((tr.z <= 0f && clipDir == false) || (tr.z > 0f && clipDir == true))
            {
                teleportObject.transform.SetPositionAndRotation(copyObject.transform.position, copyObject.transform.rotation);
            }
            material.SetInt("toggle", 0);
            copyMaterial.SetInt("toggle", 0);
            copyObject.SetActive(false);
        }
    }

    private void Clipping(Portal portal)
    {
        var m = portal.linkedPortal.transform.localToWorldMatrix * portal.transform.worldToLocalMatrix * teleportObject.transform.localToWorldMatrix;
        copyObject.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        material.SetVector("sliceCentre", portal.transform.position);
        copyMaterial.SetVector("sliceCentre", portal.linkedPortal.transform.position);
        if (clipDir)
        {
            material.SetVector("sliceNormal", portal.transform.forward);
            copyMaterial.SetVector("sliceNormal", -portal.linkedPortal.transform.forward);
        }
        else
        {
            material.SetVector("sliceNormal", -portal.transform.forward);
            copyMaterial.SetVector("sliceNormal", portal.linkedPortal.transform.forward);
        }
    }
}
