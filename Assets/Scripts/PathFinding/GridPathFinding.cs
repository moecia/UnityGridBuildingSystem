using Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class GridPathFinding
    {
        public static GridPathFinding Instance { get; private set; }

        public GridXZ<GridObject> Grid { get; private set; }

        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        private List<GridObject> openList;
        private List<GridObject> closeList;

        public GridPathFinding(GridXZ<GridObject> grid)
        {
            Instance = this;
            Grid = grid;
        }

        public List<GridObject> FindPath(int startX, int startZ, int endX, int endZ)
        {
            var startNode = Grid.GetGridObject(startX, startZ);
            var endNode = Grid.GetGridObject(endX, endZ);

            openList = new List<GridObject> { startNode };
            closeList = new List<GridObject>();

            // Init nodes
            for (int x = 0; x < Grid.Width; ++x)
            {
                for (int z = 0; z < Grid.Height; ++z)
                {
                    var pathNode = Grid.GetGridObject(x, z);
                    pathNode.G = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.CameFromNode = null;
                }
            }

            startNode.G = 0;
            startNode.H = CalculateDistanceCost(startNode, endNode);

            while (openList.Count > 0)
            {
                var currNode = GetLowestFCostNode(openList);
                if (currNode == endNode)
                {
                    return CalculatePath(endNode);
                }

                openList.Remove(currNode);
                closeList.Add(currNode);

                var neightbhourNodes = GetNeighbourList(currNode);
                foreach (var neighbourNode in neightbhourNodes)
                {
                    if (closeList.Contains(neighbourNode)) continue;
                    if (!neighbourNode.IsWalkable)
                    {
                        closeList.Add(neighbourNode);
                        continue;
                    }

                    int tentativeGCost = currNode.G + CalculateDistanceCost(currNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.G)
                    {
                        neighbourNode.CameFromNode = currNode;
                        neighbourNode.G = tentativeGCost;
                        neighbourNode.H = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();

                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }
            }

            return null;
        }

        public List<Vector3> FindPath(Vector3 startWorldPostion, Vector3 endWorldPosition)
        {
            Grid.GetXZ(startWorldPostion, out int startX, out int startZ);
            Grid.GetXZ(endWorldPosition, out int endX, out int endZ);

            var path = FindPath(startX, startZ, endX, endZ);

            if (path == null)
            {
                return null;
            }
            else
            {
                var vectorPath = new List<Vector3>();
                foreach (var pathNode in path)
                {
                    vectorPath.Add(new Vector3(pathNode.x, 0, pathNode.z) * Grid.CellSize + Vector3.one * Grid.CellSize * .5f);
                }
                return vectorPath;
            }
        }

        public GridObject GetNode(int x, int y)
        {
            return Grid.GetGridObject(x, y);
        }

        private int CalculateDistanceCost(GridObject a, GridObject b)
        {
            int xDist = Mathf.Abs(a.x - b.x);
            int zDist = Mathf.Abs(a.z - b.z);
            int remaining = Mathf.Abs(xDist - zDist);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDist, zDist) + MOVE_STRAIGHT_COST * remaining;
        }

        private GridObject GetLowestFCostNode(List<GridObject> pathNodeList)
        {
            var lowestFCostNode = pathNodeList[0];
            for (int i = 0; i < pathNodeList.Count; ++i)
            {
                if (pathNodeList[i].F < lowestFCostNode.F)
                {
                    lowestFCostNode = pathNodeList[i];
                }
            }
            return lowestFCostNode;
        }

        private List<GridObject> CalculatePath(GridObject endNode)
        {
            var path = new List<GridObject>();

            path.Add(endNode);
            var currNode = endNode;
            while (currNode.CameFromNode != null)
            {
                path.Add(currNode.CameFromNode);
                currNode = currNode.CameFromNode;
            }
            path.Reverse();
            return path;
        }

        private List<GridObject> GetNeighbourList(GridObject currNode)
        {
            var neighbours = new List<GridObject>();

            if (currNode.x - 1 >= 0)
            {
                // Left
                neighbours.Add(GetNode(currNode.x - 1, currNode.z));
                // Left Down
                if (currNode.z - 1 >= 0) neighbours.Add(GetNode(currNode.x - 1, currNode.z - 1));
                // Left Up
                if (currNode.z + 1 < Grid.Height) neighbours.Add(GetNode(currNode.x - 1, currNode.z + 1));
            }
            if (currNode.x + 1 < Grid.Width)
            {
                // Right
                neighbours.Add(GetNode(currNode.x + 1, currNode.z));
                // Right Down
                if (currNode.z - 1 >= 0) neighbours.Add(GetNode(currNode.x + 1, currNode.z - 1));
                // Right Up
                if (currNode.z + 1 < Grid.Height) neighbours.Add(GetNode(currNode.x + 1, currNode.z + 1));
            }
            // Up
            if (currNode.z + 1 < Grid.Width) neighbours.Add(GetNode(currNode.x, currNode.z + 1));
            // Down
            if (currNode.z - 1 >= 0) neighbours.Add(GetNode(currNode.x, currNode.z - 1));

            return neighbours;
        }
    }

}