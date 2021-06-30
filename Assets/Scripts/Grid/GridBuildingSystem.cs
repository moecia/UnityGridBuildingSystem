using CodeMonkey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Grid
{
    public class GridBuildingSystem: Singleton<GridBuildingSystem>
    {
        public PlaceableObjectSO PlaceableObjectSO;
        public Action OnSelectChanged;

        private GridXZ<GridObject> grid;
        private PlaceableObjectSO.Dir dir = PlaceableObjectSO.Dir.Down;
        public override void Awake()
        {
            base.Awake();
            int gridWidth = 10;
            int gridHeight = 10;
            float cellSize = 10f;

            grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
        }

        private void Update()
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
                    // TODO: print cannot build text...
                    UtilsClass.CreateWorldTextPopup("不能建在这里", mousePosition);
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
