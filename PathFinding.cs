using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;
public class PathFinding : MonoBehaviour
{
    private const int MoveCost = 1;

    private struct Cell
    {
        public int ParentX, ParentY;
        public int F, G, H;
        public void FCost() {
            F = G + H;
        }
    };

    private struct Pair
    {
        public Pair(float v, int y, int x)
        {
            Val = v;
            P1 = y;
            P2 = x;
        }
        public int P1, P2;
        public float Val;
    };
    private Cell[][] _cellGrid;
    private static int[][] _grid;
    private List<Pair> _openList;
    private List<Pair> _closedList;
    //check the Manhattan Distance between the player position and destination position
    private static int CalculateHValue(Vector2 curPos, Vector2 dest)
    {
        int x = Mathf.Abs((int)curPos.x - (int)dest.x);
        int y = Mathf.Abs((int)curPos.y - (int)dest.y);
        int remain = Mathf.Abs(x - y);
        return remain;
    }
    public PathFinding(int x, int y, int[][] grid)
    {
        _grid = grid;
        _cellGrid = new Cell[x][];
        for (int i = 0; i < _grid.Length; i++)
        {
            _cellGrid[i] = new Cell[y];
        }
    }
    //CompareVector2sAsInts()
    public Stack<Vector2> FindPath(Soldier s, Vector2 dest)
    {
        Vector2 start = s.position;
        
        _openList = new List<Pair>();
        _closedList = new List<Pair>();
        int y = (int)start.y;
        int x = (int)start.x;
        Pair destPair = new Pair(0, (int)dest.y, (int)dest.x);
        Debug.Log("Start cord: [" + x + "," + y + "]");
        Debug.Log("Dest cord: [" + dest.x + "," + dest.y + "]");
        _openList.Add(new Pair(0, y, x));
        for (int i = 0; i < _grid.Length; i++)
        {
            for (int j = 0; j < _grid[0].Length; j++)
            {
                _cellGrid[i][j] = new Cell();
                _cellGrid[i][j].G = int.MaxValue;
                //_cellGrid[i][j].h = int.MaxValue;
                _cellGrid[i][j].FCost();
                _cellGrid[i][j].ParentX = -1;
                _cellGrid[i][j].ParentY = -1;
            }
        }
        _cellGrid[y][x].ParentX = y;
        _cellGrid[y][x].ParentY = x;
        _cellGrid[y][x].G = 0;
        _cellGrid[y][x].H = CalculateHValue(start, dest);
        _cellGrid[y][x].FCost();
        while (_openList.Count > 0)
        {
            Pair curCell = LowestFCostNode(_openList);
            x = curCell.P1;
            y = curCell.P2;
            if (BattleSystem.CompareVector2sAsInts(new Vector2(x, y), dest))
            {
                return TracePath(_cellGrid, curCell);
            }
            _openList.Remove(curCell);
            _closedList.Add(curCell);
            foreach( Pair p in getNeighbors(curCell))
            {
                if (_closedList.Contains(p)) continue;
                int gCost = _cellGrid[curCell.P1][curCell.P2].G;
                int tempCost= _cellGrid[p.P1][p.P2].G;
                int tentativeGCost = gCost +1;
                //int tentiveGCost= gCost+ calculateHValue(new Vector2(curCell.p1, curCell.p2), new Vector2(p.p1, p.p2));
                if (tentativeGCost < tempCost) {
                    _cellGrid[p.P1][p.P2].ParentX = curCell.P1;
                    _cellGrid[p.P1][p.P2].ParentY = curCell.P2;
                    _cellGrid[p.P1][p.P2].G = tentativeGCost;
                    _cellGrid[p.P1][p.P2].H = CalculateHValue(new Vector2(p.P1,p.P2), dest);
                    _cellGrid[p.P1][p.P2].FCost();
                }
                if (!_openList.Contains(p))
                {
                    _openList.Add(p);
                }
            }
            
        }
        return null;
    }
    // this is a helper function that returns all of the grid positions
    // that are valid places fro the character to move!
        private List<Pair> getNeighbors(Pair p)
    {
        List<Pair> neighbors = new List<Pair>();
        if (p.P1 - 1 >= 0 && _grid[p.P1][p.P2]==0) //down
            neighbors.Add(new Pair(0, p.P1 - 1, p.P2));
        if (p.P2 - 1 >= 0 && _grid[p.P1][p.P2] == 0)//left
            neighbors.Add(new Pair(0, p.P1, p.P2-1));
        if (p.P1 < _grid.Length && _grid[p.P1][p.P2] == 0)//up
            neighbors.Add(new Pair(0, p.P1 + 1, p.P2));
        if (p.P2 < _grid[0].Length && _grid[p.P1][p.P2] == 0)//right
            neighbors.Add(new Pair(0, p.P1, p.P2+1));
        return neighbors;

    }
        //this function stores the found A* path into a stack
        private Stack<Vector2> TracePath(Cell[][] cellDeets, Pair dest)
        {
            
            Stack<Vector2> path = new Stack<Vector2>();
            int row = dest.P1;
            int col = dest.P2;
            while (!(cellDeets[row][col].ParentX == row && cellDeets[row][col].ParentY == col))
            {
            Debug.Log("[" + row + "," + col + "] : parent- ["+ cellDeets[row][col].ParentX+","+ cellDeets[row][col].ParentY+"]");
                path.Push(new Vector2(row, col));
                row = cellDeets[row][col].ParentX;
                col= cellDeets[row][col].ParentY;
            }

            return path;
        }
        private Pair LowestFCostNode(List<Pair> open) {
        Pair lowest= open[0];
        for(int i = 1; i < open.Count; i++)
        {
            int tempFCost= _cellGrid[open[i].P1][open[i].P2].F;
            int curLowFCost = _cellGrid[lowest.P1][lowest.P2].F;

            if (tempFCost < curLowFCost)
                lowest = open[i];
        }
        return lowest;
    }
}
