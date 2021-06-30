using UnityEngine;

public static class MousePositionUtils
{
    public static Vector3 MouseToTerrainPosition()
    {
        Vector3 position = Vector3.zero;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit info, 10000))
        {
            position = info.point;
        }
        return position;
    }

    public static RaycastHit CameraRay()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit info))
        {
            return info;
        }
        return new RaycastHit();
    }
}
