using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraMan : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float angleSpeed;
    [SerializeField] Transform bodyTransform;
    [SerializeField] Portal[] portals;
    public UnityAction ua;
    private float rotY, rotX;
    private Vector3 mov;
    private Vector3 minMov;
    // Start is called before the first frame update
    private void Awake()
    {
        minMov = transform.forward;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            rotY = Input.GetAxis("Mouse X") * angleSpeed * Time.smoothDeltaTime;
            rotX = -Input.GetAxis("Mouse Y") * angleSpeed * Time.smoothDeltaTime;
            bodyTransform.rotation = Quaternion.Euler(0f, bodyTransform.rotation.eulerAngles.y + rotY, 0f);
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x + rotX, 0f, 0f);
        }
        mov = transform.forward * Input.GetAxis("Vertical");
        mov += transform.right * Input.GetAxis("Horizontal");
        if (mov.x + mov.y + mov.z > 1f)
            mov.Normalize();
        mov *= moveSpeed * Time.smoothDeltaTime;

        foreach(var portal in portals)
        {
            portal.Render();
        }

        if (mov != Vector3.zero)
            minMov = mov;
        else
            minMov = mov;
        foreach(var portal in portals)
        {
            if (portal.Teleport(bodyTransform, bodyTransform.position, bodyTransform.position + minMov)) return;
        }
        bodyTransform.position += mov;
    }
}
