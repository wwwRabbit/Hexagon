using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour, ICameraControl
{
    public float minSize = 1f;
    public float maxSize = 10f;

    // �����ٶ�
    public float zoomSpeed = 1f;

    // ������ק�ı���
    private bool isDragging = false;
    private Vector3 dragOrigin;

    bool canMAndS = true;

    void Update()
    {
        // �����ֿ�������
        if (canMAndS)
        {
            ZoomCamera();

            // �����ק�����ƶ�
            DragCamera();
        }
        else
        {
            isDragging = false;
        }
        
    }

    void ZoomCamera()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newSize = Camera.main.orthographicSize - scroll * zoomSpeed;
        newSize = Mathf.Clamp(newSize, minSize, maxSize);
        Camera.main.orthographicSize = newSize;
    }

    void DragCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragOrigin = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(dragOrigin);
            transform.position -= difference;
            dragOrigin = Input.mousePosition;
        }
    }

    public void FixCamera()
    {
        canMAndS = false;
    }

    public void OpenCameraMAndS()
    {
        canMAndS = true;
    }
}
