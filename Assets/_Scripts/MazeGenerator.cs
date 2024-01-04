using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    
    [SerializeField] Transform mazeObj;

    [Header("Prefabs")]
    [SerializeField] GameObject mazeNode;
    [SerializeField] GameObject upStairsPrefab;
    [SerializeField] GameObject downStairsPrefab;

    [Space]

    [SerializeField] Vector2Int mazeDimentions;
    [SerializeField] Vector2Int startPosition;
    [SerializeField] Vector2Int endPosition;

    [Space]

    public MazeNode[] mapNodes;

    //public VisualMazeNode[,] nodes;
    public VisualMazeNode[] visualNodes;

    public bool lastVertical = true;

    Vector2Int[] visitedTest;


    public void SetX(string x)
    {
        mazeDimentions.x = int.Parse(x);
    }

    public void SetY(string y)
    {
        mazeDimentions.y = int.Parse(y);
    }


    void ClearMaze() //Prepare 
    {

        foreach (Transform child in mazeObj)
        {
            Destroy(child.gameObject);
        }

        // Prepare Things
        mapNodes = new MazeNode[mazeDimentions.x * mazeDimentions.y];
        visualNodes = new VisualMazeNode[mazeDimentions.x * mazeDimentions.y];
    }


    [ContextMenu("Spawn Nodes")]
    public void SpawnNodes()
    {
        ClearMaze();

        StartCoroutine(DelayedSpawnNodes(SetupMaze()));
        /*
        Vector3 topLeftPosition = new Vector3(-mazeDimentions.x / 2, -mazeDimentions.y / 2, 0);

        for (int x = 0; x < mazeDimentions.x; x++)
        {
            for (int y = 0; y < mazeDimentions.y; y++)
            {
                Instantiate(mazeNode, new Vector3(x + topLeftPosition.x, y + topLeftPosition.y, 0), Quaternion.identity);
            }
        }*/
    }


    [ContextMenu("Generate Maze")]
    void GenerateMaze()
    {
        // Start Array
        mapNodes = new MazeNode[mazeDimentions.x * mazeDimentions.y];

        // SSet nodes names
        for (int i = 0; i < mapNodes.Length; i++)
        {
            string name = string.Format("{0} x {1}", i % mazeDimentions.x, i / mazeDimentions.x);

            mapNodes[i] = new MazeNode(name);
        }

        

        int totalCells = mazeDimentions.x * mazeDimentions.y;
        
        Stack<Vector2Int> cellPath = new Stack<Vector2Int>();
        Stack<Vector2Int> visited = new Stack<Vector2Int>();

        // Start node
        Vector2Int startCell = startPosition;//new Vector2Int(0, mazeDimentions.y - 1);
        mapNodes[Position2DToArrayId(startCell.x, startCell.y, mazeDimentions.x)].nodeType = MazeNode.NodeType.StartingPoint;

        cellPath.Push(startCell);
        visited.Push(startCell);

        //Bool for dead ends
        bool lastPop = false;


        while (visited.Count < totalCells)
        {

            Vector2Int nextDir = NextDirectionCellBiasHorisontal(visited, mazeDimentions, cellPath.Peek().x, cellPath.Peek().y);


            lastVertical = nextDir == Vector2Int.up || nextDir == Vector2Int.down;

            if (nextDir != Vector2.zero)
            {
                //Vector2 currentCell = cellPath.Peek();

                // Get node id
                int arrayPos = Position2DToArrayId(cellPath.Peek().x, cellPath.Peek().y, mazeDimentions.x);

                // set path viable on Cell
                mapNodes[arrayPos].SetPathOpen(nextDir);

                // next cell
                cellPath.Push(cellPath.Peek() + nextDir);

                arrayPos = Position2DToArrayId(cellPath.Peek().x, cellPath.Peek().y, mazeDimentions.x);

                // set path viable on Cell
                mapNodes[arrayPos].SetPathOpen(nextDir * -1);

                //add cell to visited
                visited.Push(cellPath.Peek());

                lastPop = false;
            }
            else
            {

                if (!lastPop)
                {
                    int id = Position2DToArrayId(cellPath.Peek().x, cellPath.Peek().y, mazeDimentions.x);
                    mapNodes[id].nodeType = MazeNode.NodeType.DeadEnd;
                }
                cellPath.Pop();
                lastPop = true;
            }
        }

        //PAint last point
        mapNodes[Position2DToArrayId(cellPath.Peek().x, cellPath.Peek().y, mazeDimentions.x)].nodeType = MazeNode.NodeType.DeadEnd;

        visitedTest =   visited.ToArray();
        System.Array.Reverse(visitedTest, 0, visitedTest.Length);
    }


    [ContextMenu("Visual Biuld")]
    public void VisualBuild()
    {
        ClearMaze();
        GenerateMaze();
        StartCoroutine(DelayedSpawnNodes(SetupMazeVisual()));
    }


    IEnumerator DelayedSpawnNodes(IEnumerator OnFinished)
    {
        var waitTime = new WaitForSeconds(0.01f);

        


        Vector3 topLeftPosition = new Vector3(-((float)mazeDimentions.x) / 2, -((float)mazeDimentions.y) / 2, 0);

        for  (int y = mazeDimentions.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < mazeDimentions.x; x++)
            {
                var node = Instantiate(mazeNode, new Vector3(x, y, 0) + topLeftPosition, Quaternion.identity, mazeObj);

                node.name = (x + " " + y);

                visualNodes[Position2DToArrayId(x,y,mazeDimentions.x)] = node.GetComponent<VisualMazeNode>();

                yield return waitTime;
            }
        }

        StartCoroutine(OnFinished); //SetupMaze()
    }

    IEnumerator SetupMazeVisual()
    {
        var waitTime = new WaitForSeconds(0.08f);

        for (int i = 0; i < visitedTest.Length; i++)
        {
            Vector2Int item = visitedTest[i];

            int id = Position2DToArrayId(visitedTest[i].x, visitedTest[i].y, mazeDimentions.x);

            SetVisualNodeOpen(mapNodes[id], visualNodes[id]);

            //visualNodes[id].ColorCell(Color.blue);
            VisualColorNode(mapNodes[id], visualNodes[id]);

            yield return waitTime;
        }
    }


    IEnumerator SetupMaze()
    {
        var waitTime = new WaitForSeconds(0.08f);


        int totalCells = mazeDimentions.x* mazeDimentions.y;

        Vector2Int startCell = new Vector2Int(0, mazeDimentions.y-1);  //Vector2Int.zero;   // Start cell = 0,0
        // End Cell = 10,10

        Stack <Vector2Int> cellPath = new Stack<Vector2Int>();
        Stack<Vector2Int> visited = new Stack<Vector2Int>();


        cellPath.Push(startCell);
        visited.Push(startCell);

        bool lastPop = false;

        ColorNode(Color.green, startCell);

        // Do loop while all cells are not visited yet
        while (visited.Count < totalCells)
        {

            Vector2Int nextDir = NextDirectionCellBiasHorisontal(visited, mazeDimentions, cellPath.Peek().x, cellPath.Peek().y);
            lastVertical = nextDir == Vector2Int.up || nextDir == Vector2Int.down;

            if (nextDir != Vector2.zero)
            {
                //Vector2 currentCell = cellPath.Peek();

                // Get node id
                int arrayPos = Position2DToArrayId(cellPath.Peek().x, cellPath.Peek().y,mazeDimentions.x);

                // set path viable on Cell
                visualNodes[arrayPos].SetPathOpen(nextDir);
                

                // next cell
                cellPath.Push(cellPath.Peek() + nextDir);

                arrayPos = Position2DToArrayId(cellPath.Peek().x, cellPath.Peek().y, mazeDimentions.x);

                // set path viable on Cell
                visualNodes[arrayPos].SetPathOpen(nextDir * -1);

                ColorNode(Color.blue, cellPath.Peek());

                //add cell to visited
                visited.Push(cellPath.Peek());

                lastPop = false;

                yield return waitTime;
            }
            else
            {
                if (!lastPop)
                {
                    ColorNode(Color.red, cellPath.Peek());
                    yield return waitTime;
                }
                cellPath.Pop();
                lastPop = true;
                
            }

        }


        //PAint last point
        ColorNode(Color.red, cellPath.Peek());

        yield return null;
    }


    public void GenerateStairs()
    {
        GenerateStairsUp();
        GenerateStairsDown();
    }

    void GenerateStairsUp()
    {
        // Loop Map
        for (int y = mazeDimentions.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < mazeDimentions.x; x++)
            {
                if(CheckPatternUpStairs(new Vector2Int(x,y), mapNodes))
                {
                    Instantiate(upStairsPrefab, visualNodes[ Position2DToArrayId( x, y , mazeDimentions.x)].transform.position, Quaternion.identity, mazeObj);
                }
            }
        }
    }

    void GenerateStairsDown()
    {
        // Loop Map
        for (int y = mazeDimentions.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < mazeDimentions.x; x++)
            {
                if (CheckPatternDownStairs(new Vector2Int(x, y), mapNodes))
                {
                    Instantiate(downStairsPrefab, visualNodes[Position2DToArrayId(x, y, mazeDimentions.x)].transform.position, Quaternion.identity, mazeObj);
                }
            }
        }
    }

    bool CheckPatternUpStairs(Vector2Int position, MazeNode[] nodes)
    {
        /* Pattern is
         *     x+1 y+1 left true down false
         *  xy up True Down False
         */

        if (position.x < mazeDimentions.x - 1 && position.y >= 0)   // xy is valid
        {
            if (position.x >= 0 && position.y < mazeDimentions.y - 1)   // x+1 y+1 is valid
            {

                int pos1 = Position2DToArrayId(position.x, position.y, mazeDimentions.x);
                int pos2 = Position2DToArrayId(position.x + 1, position.y + 1, mazeDimentions.x);

                return (nodes[pos1].up && !nodes[pos1].down)          // up True Down False
                    && (nodes[pos2].left && !nodes[pos2].down); // left true down false
            }
        }
        return false;
    }

    bool CheckPatternDownStairs(Vector2Int position, MazeNode[] nodes)
    {
        /* Pattern is
         *     x-1 y+1 left true down false
         *  xy up True Down False
         */

        if (position.x < mazeDimentions.x && position.y >= 0)   // xy is valid
        {
            if (position.x > 0 && position.y < mazeDimentions.y - 1)   // x-1 y+1 is valid
            {
                //Debug.Log(string.Format("{0} {1}", position.x, position.y));
                //Debug.Log(string.Format("{0} {1}", position.x + 1, position.y - 1));
                //Debug.Log("---");

                int pos1 = Position2DToArrayId(position.x, position.y, mazeDimentions.x);
                int pos2 = Position2DToArrayId(position.x - 1, position.y + 1, mazeDimentions.x);

                return (nodes[pos1].up && !nodes[pos1].down)          // up True Down False
                    && (nodes[pos2].right && !nodes[pos2].down); // right true down false
            }
        }
        return false;
    }



    // Check and return direction 
    Vector2Int NextDirectionCell(Stack<Vector2Int> visited, Vector2Int mazeDimentions, int x, int y)
    {

        List<Vector2Int> possibleDirections = new List<Vector2Int>();

        //check if north is avaliable
        if (y > 0 && !visited.Contains(new Vector2Int(x, y - 1)))
        {
            possibleDirections.Add(Vector2Int.down);
        }

        //check East
        if (x < mazeDimentions.x - 1 && !visited.Contains(new Vector2Int(x + 1, y)))
        {
            possibleDirections.Add(Vector2Int.right);
        }

        //check south
        if (y < mazeDimentions.y - 1 && !visited.Contains(new Vector2Int(x, y + 1)))
        {
            possibleDirections.Add(Vector2Int.up);
        }

        //check west
        if (x > 0 && !visited.Contains(new Vector2Int(x - 1, y)))
        {
            possibleDirections.Add(Vector2Int.left);
        }


        if (possibleDirections.Count > 0)
        {
            int random = UnityEngine.Random.Range(0, possibleDirections.Count);
            return possibleDirections[random];  // return random direction avaliable
        }
        else
            return Vector2Int.zero;    // No direction avaliable
    }


    Vector2Int NextDirectionCellBiasHorisontal(Stack<Vector2Int> visited, Vector2Int mazeDimentions, int x, int y)
    {

        List<Vector2Int> possibleDirections = new List<Vector2Int>();

        //check if north is avaliable
        if (y < mazeDimentions.y - 1 && !visited.Contains(new Vector2Int(x, y + 1)) && !lastVertical)  // Add prevent free fall
        {
            possibleDirections.Add(Vector2Int.up);
        }

        //check south
        if (y > 0 && !visited.Contains(new Vector2Int(x, y - 1)) && !lastVertical)
        {
            possibleDirections.Add(Vector2Int.down);
        }

        //check East
        if (x < mazeDimentions.x - 1 && !visited.Contains(new Vector2Int(x + 1, y)))
        {
            possibleDirections.Add(Vector2Int.right);
            possibleDirections.Add(Vector2Int.right);
        }



        //check west
        if (x > 0 && !visited.Contains(new Vector2Int(x - 1, y)))
        {
            possibleDirections.Add(Vector2Int.left);
            possibleDirections.Add(Vector2Int.left);
        }


        if (possibleDirections.Count > 0)
        {
            int random = UnityEngine.Random.Range(0, possibleDirections.Count);
            return possibleDirections[random];  // return random direction avaliable
        }
        else
            return Vector2Int.zero;    // No direction avaliable
    }

       // Utils
    int Position2DToArrayId(int x, int y, int xDimention)
    {
        return y * xDimention + x;
    }

    void SetVisualNodeOpen(MazeNode node, VisualMazeNode visual)
    {
        if (node.up) visual.SetPathOpen(Vector2Int.up);
        if (node.down) visual.SetPathOpen(Vector2Int.down);
        if (node.left) visual.SetPathOpen(Vector2Int.left);
        if (node.right) visual.SetPathOpen(Vector2Int.right);

    }

    void ColorNode(Color color, Vector2Int position)
    {
        visualNodes[Position2DToArrayId(position.x, position.y, mazeDimentions.x)].ColorCell(color);
    }

    void VisualColorNode(MazeNode node, VisualMazeNode visual)
    {
        Color targetColor = Color.blue;

        if (node.nodeType == MazeNode.NodeType.DeadEnd) targetColor = Color.red;

        if (node.nodeType == MazeNode.NodeType.StartingPoint) targetColor = Color.green;

        visual.ColorCell(targetColor);
    }

}




[System.Serializable]
public class MazeNode
{
    [HideInInspector] public string name ="tt";

    public bool up;
    public bool right;
    public bool down;
    public bool left;

    [Space]

    public bool visited = false;
    public NodeType nodeType;


    public MazeNode(string name)
    {
        this.name = name;
    }

    public void SetPathOpen(Vector2Int dir)
    {
        switch (dir)
        {
            case Vector2Int v when v.Equals(Vector2Int.up):
                up = true;
                break;
            case Vector2Int v when v.Equals(Vector2Int.down):
                down = true;
                break;
            case Vector2Int v when v.Equals(Vector2Int.left):
                left = true;
                break;
            case Vector2Int v when v.Equals(Vector2Int.right):
                right = true;
                break;
        }
    }

    public enum NodeType
    {
        Basic,
        StartingPoint,
        DeadEnd,
        StairsUp,
        StairsDown
    }
}