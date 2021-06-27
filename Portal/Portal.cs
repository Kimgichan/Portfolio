using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;
    public MeshRenderer screen;
    Camera portalCam;
    RenderTexture viewTexture;
    [SerializeField] Vector2 gizmoQuadSize;
    [SerializeField] Color gizmoColor;
    //[SerializeField] int recursionLimit;
    //public float nearClipOffset = 0.05f;
    //public float nearClipLimit = 0.2f;

    void Awake()
    {
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false;
    }

    void CreateViewTexture()
    {
        if(viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
        {
            if(viewTexture != null)
            {
                viewTexture.Release();
            }
            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            //Render the view from the portal camera to the view texture
            portalCam.targetTexture = viewTexture;
            //Display the view texture on the screen of the linked portal
            linkedPortal.screen.material.SetTexture("_MainTex", viewTexture);
        }
    }

    static bool VisibleFromCamera(Renderer renderer)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    //void SetNearClipPlane()
    //{
    //    // Learning resource:
    //    // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
    //    Transform clipPlane = transform;
    //    int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - portalCam.transform.position));

    //    Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
    //    Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
    //    float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

    //    // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
    //    if (Mathf.Abs(camSpaceDst) > nearClipLimit)
    //    {
    //        Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

    //        // Update projection based on new clip plane
    //        // Calculate matrix with player cam so that player camera settings (fov, etc) are used
    //        portalCam.projectionMatrix = Camera.main.CalculateObliqueMatrix(clipPlaneCameraSpace);
    //    }
    //    else
    //    {
    //        portalCam.projectionMatrix = Camera.main.projectionMatrix;
    //    }
    //}

    //Called just before player camera is rendered
    public void Render()
    {
        if (!VisibleFromCamera(linkedPortal.screen))
        {
            var testTexture = new Texture2D(1, 1);
            testTexture.SetPixel(0, 0, Color.black);
            testTexture.Apply();
            linkedPortal.screen.material.SetTexture("_MainTex", testTexture);
            return;
        }
        linkedPortal.screen.material.SetTexture("_MainTex", viewTexture);
        screen.enabled = false;
        CreateViewTexture();

        //Make portal cam postion and rotation the same relative to this portal as player cam relative to linked portal
        //Matrix4x4 localToWorldMatrix = Camera.main.transform.localToWorldMatrix;
        //Matrix4x4[] matrices = new Matrix4x4[recursionLimit];
        //for(int i = 0; i<recursionLimit; i++)
        //{
        //    localToWorldMatrix = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * localToWorldMatrix;
        //    matrices[recursionLimit - i - 1] = localToWorldMatrix;
        //}

        //screen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

        //for(int i = 0; i<recursionLimit; i++)
        //{
        //    portalCam.transform.SetPositionAndRotation(matrices[i].GetColumn(3), matrices[i].rotation);
        //    SetNearClipPlane();
        //    portalCam.Render();
        //}
        //screen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        var m = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * Camera.main.transform.localToWorldMatrix;
        portalCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
        portalCam.Render();
        screen.enabled = true;
    }

    public bool Teleport(Transform tr, Vector3 worldStart, Vector3 worldDest)
    {
        var start = transform.InverseTransformPoint(worldStart);
        var dest = transform.InverseTransformPoint(worldDest);
        Vector3 dir = dest - start;
        float k = -(start.z / dir.z);
        if (k >= 0f && k <= 1f)
        {
            float x = start.x + k * dir.x;
            float y = start.y + k * dir.y;
            if (-gizmoQuadSize.x < x && x < gizmoQuadSize.x &&
                -gizmoQuadSize.y < y && y < gizmoQuadSize.y)
            {
                tr.position = linkedPortal.transform.TransformPoint(dest);
                var newDir = linkedPortal.transform.TransformDirection(transform.InverseTransformDirection(tr.forward));
                tr.rotation = Quaternion.LookRotation(newDir);
                return true;
            }
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (gizmoQuadSize.x > 0f && gizmoQuadSize.y > 0f)
        {
            Vector3 up = transform.up * gizmoQuadSize.y;
            Vector3 right = transform.right * gizmoQuadSize.x;
            Gizmos.color = gizmoColor;
            Gizmos.DrawLine(transform.position - up + right, transform.position + up + right);
            Gizmos.DrawLine(transform.position - up - right, transform.position + up - right);
            Gizmos.DrawLine(transform.position - up + right, transform.position - up - right);
            Gizmos.DrawLine(transform.position + up - right, transform.position + up + right);
        }
    }
}
