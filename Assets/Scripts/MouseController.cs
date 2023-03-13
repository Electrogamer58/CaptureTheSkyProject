using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    private GridMovementActions gMovementActions;
    private Camera mainCamera;

    private void Awake()
    {
        gMovementActions = new GridMovementActions();
        mainCamera = Camera.main;
    }
    private void OnEnable()
    {
        gMovementActions.Enable();
    }
    private void OnDisable()
    {
        gMovementActions.Disable();
    }
    private void Start()
    {
        gMovementActions.Testing.Click.started += ctx => Click();
    }

    private void Click()
    {
        DetectObject();
    }
    private void DetectObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(gMovementActions.Testing.Position.ReadValue<Vector2>());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                NodeObject nodeObject = hit.collider.GetComponent<NodeObject>();
                if (nodeObject != null)
                {
                    //Debug.Log(pointObject.name);
                    nodeObject.SelectThisNode();
                }
            }
        }
    }
}
