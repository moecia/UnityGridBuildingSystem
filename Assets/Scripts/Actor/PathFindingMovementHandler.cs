using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;
using Grid;

public class PathFindingMovementHandler : MonoBehaviour
{
    [SerializeField] private float speed = 40f;
    [SerializeField] private float stopDistance = 1f;

    private int currentPathIndex;
    private List<Vector3> path;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!GridBuildingSystem.Instance.IsBuildingMode)
        {
            HandleMovement();

            if (Input.GetMouseButtonDown(0))
            {
                SetTargetPosition(MousePositionUtils.MouseToTerrainPosition());
            }
        }
    }

    private void SetTargetPosition(Vector3 targetPosition)
    {
        currentPathIndex = 0;
        path = GridPathFinding.Instance.FindPath(transform.position, targetPosition);

        if (path != null && path.Count > 1)
        {
            path.RemoveAt(0);
        }
    }

    private void StopMoving()
    {
        path = null;
    }

    private void HandleMovement()
    {
        if (path != null)
        {
            var currPath = path[currentPathIndex];
            var targetPosition = new Vector3(currPath.x, transform.position.y, currPath.z);

            for (int i = currentPathIndex; i < path.Count - 1; ++i)
            {
                Debug.DrawLine(new Vector3(path[i].x, transform.position.y, path[i].z),
                    new Vector3(path[i + 1].x, transform.position.y, path[i + 1].z),
                    Color.green);
            }

            if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
            {
                var moveDir = (targetPosition - transform.position).normalized;
                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= path.Count)
                {
                    StopMoving();
                }
            }
        }
    }
}
