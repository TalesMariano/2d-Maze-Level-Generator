using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualMazeNode : MonoBehaviour
{
    [SerializeField] SpriteRenderer bgSprite;
    [Space]
    [SerializeField] GameObject northWall;
    [SerializeField] GameObject eastWall;
    [SerializeField] GameObject southWall;
    [SerializeField] GameObject westWall;

    [Tooltip("0 - North, 1- East, 2- South, 3-West")]
    public bool[] paths = new bool[4];


    [ContextMenu("test")]
    void test()
    {
        SetPathOpen(Vector2Int.down);
    }

    public void ColorCell(Color color)
    {
        bgSprite.color = color;
    }

    public void SetPathOpen(Vector2Int dir)
    {
        switch (dir)
        {
            case Vector2Int v when v.Equals(Vector2Int.up):
                northWall.SetActive(false);
                paths[0] = true;
                break;
            case Vector2Int v when v.Equals(Vector2Int.down):
                southWall.SetActive(false);
                paths[2] = true;
                break;
            case Vector2Int v when v.Equals(Vector2Int.left):
                westWall.SetActive(false);
                paths[3] = true;
                break;
            case Vector2Int v when v.Equals(Vector2Int.right):
                eastWall.SetActive(false);
                paths[1] = true;
                break;

        }

        //UpdVisualWall();
    }


    public bool IsPathOpen(Vector2Int dir)
    {
        switch (dir)
        {
            case Vector2Int v when v.Equals(Vector3.up):
                return paths[0];

            case Vector2Int v when v.Equals(Vector3.down):
                return paths[2];

            case Vector2Int v when v.Equals(Vector3.left):
                return paths[3];

            case Vector2Int v when v.Equals(Vector3.right):
                return paths[1];

            default:
                return false;

        }
    }
}
