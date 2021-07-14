using Newtonsoft.Json;

namespace Grid
{
    public class GridObject
    {
        public bool CanBuild { get { return placeableObject == null; } }
        [JsonIgnore]
        public PlaceableObject PlaceableObject
        {
            get => placeableObject;
            set
            {
                IsWalkable = false;
                placeableObject = value;
                grid.TriggerGridObjectChanged(x, z);
            }
        }
        public int x;
        public int z;
        public string PlaceableObjectName;
        public PlaceableObjectSO.Dir Direction;

        private GridXZ<GridObject> grid;
        private PlaceableObject placeableObject;

        // Path Finding 
        // Cost from the start node
        [JsonIgnore]
        public int G;
        // Heuristic cost to reach the end node(won't consider obstacle)
        [JsonIgnore]
        public int H;
        [JsonIgnore]
        public int F;
        [JsonIgnore]
        public GridObject CameFromNode;
        [JsonIgnore]
        public bool IsWalkable;

        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
            IsWalkable = true;
        }

        public override string ToString()
        {
            return $"{x}, {z}";
        }

        public void CalculateFCost()
        {
            F = G + H;
        }

        public void ClearPlaceableObject()
        {
            IsWalkable = true;
            placeableObject = null;
            PlaceableObjectName = null;
            Direction = PlaceableObjectSO.Dir.Down;
            grid.TriggerGridObjectChanged(x, z);
        }
    }
}
