using CodeMonkey.Utils;
using PathFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Grid
{
    public class GridBuildingSystem : Singleton<GridBuildingSystem>
    {
        public PlaceableObjectSO PlaceableObjectSO;
        public int rowCount = 10;
        public int columnCount = 10;
        public int cellSize = 5;
        public Vector3 StartOrigin = Vector3.zero;
        public bool ShowDebug = false;

        public Action OnSelectChanged;
        public Action<bool> OnBuildingModeChanged;

        public bool IsBuildingMode { get; set; } = false;

        private GridXZ<GridObject> grid;
        private PlaceableObjectSO.Dir dir = PlaceableObjectSO.Dir.Down;

        private GridPathFinding pf;

        public override void Awake()
        {
            base.Awake();

            grid = new GridXZ<GridObject>(rowCount,
                columnCount,
                cellSize,
                StartOrigin,
                (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z),
                ShowDebug);
            pf = new GridPathFinding(grid);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                IsBuildingMode = !IsBuildingMode;
                OnBuildingModeChanged.Invoke(IsBuildingMode);
            }

            if (IsBuildingMode)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var mousePosition = MousePositionUtils.MouseToTerrainPosition();
                    grid.GetXZ(mousePosition, out int x, out int z);

                    var gridPosList = PlaceableObjectSO.GetGridPositionList(new Vector2Int(x, z), dir);
                    var canBuild = true;
                    foreach (var gridPos in gridPosList)
                    {
                        if (!grid.GetGridObject(gridPos.x, gridPos.y).CanBuild)
                        {
                            canBuild = false;
                            break;
                        }
                    }
                    if (canBuild)
                    {
                        var rotOffset = PlaceableObjectSO.GetRotationOffset(dir);
                        var placedWorldPos = grid.GetWorldPosition(x, z) + new Vector3(rotOffset.x, 0, rotOffset.y) * grid.CellSize;

                        var placedObject = PlaceableObject.Create(
                            placedWorldPos,
                            new Vector2Int(x, z), dir,
                            PlaceableObjectSO);
                        foreach (var gridPos in gridPosList)
                        {
                            grid.GetGridObject(gridPos.x, gridPos.y).PlaceableObject = placedObject;
                        }
                    }
                    else
                    {
                        UtilsClass.CreateWorldTextPopup("Cannot build here", mousePosition);
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    var gridObject = grid.GetGridObject(MousePositionUtils.MouseToTerrainPosition());
                    var placedObject = gridObject.PlaceableObject;
                    if (placedObject != null)
                    {
                        placedObject.DestroySelf();
                        var gridPosList = placedObject.GetGridPositionList();
                        foreach (var gridPos in gridPosList)
                        {
                            grid.GetGridObject(gridPos.x, gridPos.y).PlaceableObject = placedObject;
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    dir = PlaceableObjectSO.GetNextDir(dir);
                }
            }
        }

        public Quaternion GetCurrentBuildingRotation()
        {
            if (PlaceableObjectSO != null)
            {
                return Quaternion.Euler(0, PlaceableObjectSO.GetRotationAngle(dir), 0);
            }
            else
            {
                return Quaternion.identity;
            }
        }

        public Vector3 GetMouseWorldSnappedPosition()
        {
            var mousePosition = MousePositionUtils.MouseToTerrainPosition();
            grid.GetXZ(mousePosition, out int x, out int z);

            if (PlaceableObjectSO != null)
            {
                var rotationOffset = PlaceableObjectSO.GetRotationOffset(dir);
                return grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.CellSize;
            }
            else
            {
                return mousePosition;
            }
        }
    }
}