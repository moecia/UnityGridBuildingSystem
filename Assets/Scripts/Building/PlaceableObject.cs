using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlaceableObject : MonoBehaviour
{
    public PlaceableObjectSO placeableObjectSO;
    public PlaceableObjectSO.Dir dir;
    private Vector2Int origin;

    public static PlaceableObject Create(Vector3 worldPosition, Vector2Int origin, PlaceableObjectSO.Dir dir, PlaceableObjectSO placeableObjectSO)
    {
        var placedObjectTransform = Instantiate(
            placeableObjectSO.Prefab,
            worldPosition,
            Quaternion.Euler(0, placeableObjectSO.GetRotationAngle(dir), 0));
        var placeableObject = placedObjectTransform.GetComponent<PlaceableObject>();

        placeableObject.placeableObjectSO = placeableObjectSO;
        placeableObject.origin = origin;
        placeableObject.dir = dir;

        // Build effects
        placedObjectTransform.DOShakeScale(.5f, .2f, 10, 90, true);

        return placeableObject;
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return placeableObjectSO.GetGridPositionList(origin, dir);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
