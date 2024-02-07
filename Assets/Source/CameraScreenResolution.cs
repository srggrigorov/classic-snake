using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraScreenResolution : MonoBehaviour
{
    [SerializeField]
    private bool maintainWidth = true;
    [Range(-1, 1)]
    [SerializeField]
    private int adaptPosition;
    private float defaultWidth, defaultHeight;
    private Camera targetCamera;

    private Vector3 cameraPos;

    private void OnEnable()
    {
        targetCamera ??= GetComponent<Camera>();
        cameraPos = targetCamera.transform.position;
        defaultHeight = targetCamera.orthographicSize;
        defaultWidth = targetCamera.orthographicSize * targetCamera.aspect;
    }

    private void Update()
    {
        if (maintainWidth)
        {
            targetCamera.orthographicSize = defaultWidth / targetCamera.aspect;
            targetCamera.transform.position = new Vector3(cameraPos.x,
                adaptPosition * (defaultHeight - targetCamera.orthographicSize), cameraPos.z);
        }
        else
        {
            targetCamera.transform.position =
                new Vector3(adaptPosition * (defaultWidth - targetCamera.orthographicSize * targetCamera.aspect),
                    cameraPos.y, cameraPos.z);
        }
    }
}