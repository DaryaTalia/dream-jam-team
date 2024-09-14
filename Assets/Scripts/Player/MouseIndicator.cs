using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MouseIndicator : MonoBehaviour
{

    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private Camera camera;
    
    private Vector3 lastPosition;

    [SerializeField] private LayerMask layerCheckMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveMouseLoc();
    }

    void MoveMouseLoc()
    {
        Vector3 cursorPos = GetSelectedMapPosition();
        cursorPos.y = 1f;
        mouseIndicator.transform.position = cursorPos;
    }

    private Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        //mousePos.y = Camera.main.nearClipPlane;
        Ray ray = camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 3000 /* length of ray needs to reach back of map */ , layerCheckMask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}
