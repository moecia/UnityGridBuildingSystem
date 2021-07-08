using Grid;
using UnityEngine;
using UnityEngine.AI;

public class BuildingPreview : MonoBehaviour
{
    [SerializeField] private Material previewMaterial;

    private Transform visual;
    private PlaceableObjectSO placeableObjectSO;
    private Mesh previewMesh;

    private void Start()
    {
        if (GridBuildingSystem.Instance.IsBuildingMode)
        {
            RefreshVisual();
        }
        GridBuildingSystem.Instance.OnBuildingModeChanged += BuildingModeChangedHandler;
    }

    private void LateUpdate()
    {
        if (GridBuildingSystem.Instance.IsBuildingMode)
        {
            var targetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPosition();
            targetPosition.y = 1f;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingSystem.Instance.GetCurrentBuildingRotation(), Time.deltaTime * 15f);
        }
    }

    private void RefreshVisual()
    {
        DestroyPreview();

        var placeableObjectSO = GridBuildingSystem.Instance.PlaceableObjectSO;
        if (placeableObjectSO != null)
        {
            visual = Instantiate(placeableObjectSO.Prefab.GetChild(0), Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            var meshes = visual.GetComponentsInChildren<MeshRenderer>();
            foreach (var mesh in meshes)
            {
                mesh.material = previewMaterial;
            }
            var navObstacles = visual.GetComponentsInChildren<NavMeshObstacle>();
            foreach (var navObstacle in navObstacles)
            {
                navObstacle.enabled = false;
            }
        }
    }

    private void DestroyPreview()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }
    }

    private void BuildingModeChangedHandler(bool IsBuildingMode)
    {
        if (IsBuildingMode)
        {
            RefreshVisual();
        }
        else
        {
            DestroyPreview();
        }
    }
}
