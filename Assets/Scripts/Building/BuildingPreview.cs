using Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    [SerializeField] private Material previewMaterial;

    private Transform visual;
    private PlaceableObjectSO placeableObjectSO;
    private Mesh previewMesh;

    private void Start()
    {
        RefreshVisual();
    }

    private void LateUpdate()
    {
        var targetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPosition();
        targetPosition.y = 1f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
        transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingSystem.Instance.GetCurrentBuildingRotation(), Time.deltaTime * 15f);
    }

    private void RefreshVisual()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }

        var placeableObjectSO = GridBuildingSystem.Instance.PlaceableObjectSO;
        if (placeableObjectSO != null)
        {
            visual = Instantiate(placeableObjectSO.Prefab, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            visual.GetComponentInChildren<MeshRenderer>().material = previewMaterial;
        }
    }
}
