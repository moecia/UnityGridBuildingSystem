using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class GridObject
    {
        public bool CanBuild { get { return placeableObject == null; } }
        public PlaceableObject PlaceableObject
        {
            get => placeableObject;
            set
            {
                placeableObject = value;
                grid.TriggerGridObjectChanged(x, z);
            }
        }

        private GridXZ<GridObject> grid;
        private int x;
        private int z;
        private PlaceableObject placeableObject;

        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public override string ToString()
        {
            return $"{x}, {z}";
        }

        public void ClearPlaceableObject()
        {
            placeableObject = null;
            grid.TriggerGridObjectChanged(x, z);
        }
    }
}
