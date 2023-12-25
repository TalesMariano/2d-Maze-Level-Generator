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

    public VisualMazeNode[,] nodes;

    public bool lastVertical = true;

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
        nodes = new VisualMazeNode[mazeDimentions.x, mazeDimentions.y];
    }


    [ContextMenu("Spawn Nodes")]
    public void SpawnNodes()
    {
        ClearMaze();

        StartCoroutine(DelayedSpawnNodes());
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

    IEnumerator DelayedSpawnNodes()
    {
        var waitTime = new WaitForSeconds(0.01f);

        


        Vector3 topLeftPosition = new Vector3(-((float)mazeDimentions.x) / 2, -((float)mazeDimentions.y) / 2, 0);

        for  (int y = mazeDimentions.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < mazeDimentions.x; x++)
            {
                var node = Instantiate(mazeNode, new Vector3(x, y, 0) + topLeftPosition, Quaternion.identity, mazeObj);

                node.name = (x + " " + y);

                nodes[x,y] = node.GetComponent<VisualMazeNode>();

                yield return waitTime;
            }
        }

        StartCoroutine(SetupMaze());
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

                // set path viable on Cell
                nodes[cellPath.Peek().x, cellPath.Peek().y].SetPathOpen(nextDir);
                

                // next cell
                cellPath.Push(cellPath.Peek() + nextDir);

                // set path viable on Cell
                nodes[cellPath.Peek().x, cellPath.Peek().y].SetPathOpen(nextDir * -1);

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
                if(CheckPatternUpStairs(new Vector2Int(x,y), nodes))
                {
                    Instantiate(upStairsPrefab, nodes[x, y].transform.position, Quaternion.identity, mazeObj);
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
                if (CheckPatternDownStairs(new Vector2Int(x, y), nodes))
                {
                    Instantiate(downStairsPrefab, nodes[x, y].transform.position, Quaternion.identity, mazeObj);
                }
            }
        }
    }

    bool CheckPatternUpStairs(Vector2Int position, VisualMazeNode[,] nodes)
    {
        /* Pattern is
         *     x+1 y+1 left true down false
         *  xy up True Down False
         */

        if (position.x < nodes.GetLength(0)-1 && position.y >= 0)   // xy is valid
        {
            if (position.x >= 0 && position.y < nodes.GetLength(1) - 1)   // x+1 y+1 is valid
            {

                return (nodes[position.x, position.y].paths[0] && !nodes[position.x, position.y].paths[2])          // up True Down False
                    && (nodes[position.x + 1, position.y + 1].paths[3] && !nodes[position.x + 1, position.y + 1].paths[2]); // left true down false
            }
        }
        return false;
    }

    bool CheckPatternDownStairs(Vector2Int position, VisualMazeNode[,] nodes)
    {
        /* Pattern is
         *     x-1 y+1 left true down false
         *  xy up True Down False
         */

        if (position.x < nodes.GetLength(0)  && position.y >= 0)   // xy is valid
        {
            if (position.x > 0 && position.y < nodes.GetLength(1)-1)   // x-1 y+1 is valid
            {
                Debug.Log(string.Format("{0} {1}", position.x, position.y));
                Debug.Log(string.Format("{0} {1}", position.x + 1, position.y - 1));
                Debug.Log("---");


                return (nodes[position.x, position.y].paths[0] && !nodes[position.x, position.y].paths[2])          // up True Down False
                    && (nodes[position.x - 1, position.y + 1].paths[1] && !nodes[position.x - 1, position.y + 1].paths[2]); // right true down false
            }
        }
        return false;
    }

    void ColorNode(Color color, Vector2Int position)
    {
        nodes[position.x, position.y].ColorCell(color);
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
            int random = Random.Range(0, possibleDirections.Count);
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
            int random = Random.Range(0, possibleDirections.Count);
            return possibleDirections[random];  // return random direction avaliable
        }
        else
            return Vector2Int.zero;    // No direction avaliable
    }
}




[System.Serializable]
public class MazeNode
{
    public bool visited = false;
    public Vector4 vector4;
}
