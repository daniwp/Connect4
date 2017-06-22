using UnityEngine;
using System.Collections;

public class BrickManager : MonoBehaviour
{

    private Vector3 mousePosition;
    private float height = 3.26f;
    public bool isPlayerOneTurn;
    public bool isActive;
    private GameObject curBrick;
    public GameObject p1Brick;
    public GameObject p2Brick;
    [Range(0f, 5f)]
    public float distanceX = 1.36f;
    private float slot1x = -4.1f;
    public float[] gridWidth;
    private ArrayList dropPositions = new ArrayList();

    private enum Field
    {
        empty,
        playerone,
        playertwo
    }
    private Field[,] grid;

    void Start()
    {
        dropPositions.Add(new Vector3(-4.1f, 3.26f, 1));
        dropPositions.Add(new Vector3(-2.715f, 3.26f, 1));
        dropPositions.Add(new Vector3(-1.33f, 3.26f, 1));
        dropPositions.Add(new Vector3(0.05500007f, 3.26f, 1));
        dropPositions.Add(new Vector3(1.44f, 3.26f, 1));
        dropPositions.Add(new Vector3(2.825f, 3.26f, 1));
        dropPositions.Add(new Vector3(4.21f, 3.26f, 1));

        transform.position = new Vector3(0, height, transform.position.z);
        gridWidth = new float[] {slot1x,slot1x+distanceX ,slot1x+distanceX*2,slot1x+distanceX*3,slot1x+distanceX*4,slot1x+distanceX*5,
            slot1x+distanceX*6 };
        grid = new Field[7, 6];
    }

    void Update()
    {
        if (isActive)
        {
            mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 newPosition = new Vector3(mousePosition.x, height, 1);
            if (newPosition.x <= -4.15f)
            {
                newPosition.x = -4.15f;
            }
            if (newPosition.x >= 4.15f)
            {
                newPosition.x = 4.15f;
            }
            curBrick.transform.position = newPosition;
            int column = CheckBoundaryAndSetPosX();
            /////////////////////
            bool hasNextMove = false;
            bool isLoosing = false;

            if (!isPlayerOneTurn)
            {
                for (int times = 3; times > 0 ; times--)
                {
                    if (hasNextMove || isLoosing)
                    {
                        break;
                    }
                    for (int i = 6; i >= 0; i--)
                    {
                        if (CheckIfWinning(i, times))
                        {
                            column = i;
                            hasNextMove = true;
                            break;
                        } else if (CheckIfLoosing(i))
                        {
                            column = i;
                            isLoosing = true;
                            break;
                        }
                    }
                }

                if (hasNextMove)
                {
                    Debug.Log("IsWinningMove");
                    DropBrickAtPos(column);
                    isActive = false;
                    bool isGameOver = CheckValidPos(column);
                    if (!isGameOver)
                        StartCoroutine(GetComponent<GameController>().endPlayerTurn());
                } else if (isLoosing)
                {
                    Debug.Log("IsLoosingMove");
                    DropBrickAtPos(column);
                    isActive = false;
                    bool isGameOver = CheckValidPos(column);
                    if (!isGameOver)
                        StartCoroutine(GetComponent<GameController>().endPlayerTurn());
                }
                else
                {
                    Debug.Log("DROPPING RANDOM BLOCK!");
                    column = DropRandomBrick();
                    isActive = false;
                    bool isGameOver = CheckValidPos(column);
                    if (!isGameOver)
                        StartCoroutine(GetComponent<GameController>().endPlayerTurn());
                }

            }

            if (Input.GetMouseButtonDown(0))
            {
                DropBrick();
                isActive = false;
                bool isGameOver = CheckValidPos(column);
                if (!isGameOver)
                    StartCoroutine(GetComponent<GameController>().endPlayerTurn());
            }
        }

    }

    public bool CheckValidPos(int column)
    {
        for (int i = grid.GetLength(1) - 1; i >= 0; i--)
        {
            if (grid[column, i] == Field.empty)
            {
                grid[column, i] = isPlayerOneTurn ? Field.playerone : Field.playertwo;
                //Debug.Log("Placing player brick at depth: " + (i + 1));
                if (LightAlgorithm(column, i, 3))
                {
                    declareWinner(isPlayerOneTurn);
                    return true;
                }
                break;
            }
        }
        return false;
    }

    public bool CheckIfWinning(int column, int times)
    {
        for (int i = grid.GetLength(1) - 1; i >= 0; i--)
        {
            if (grid[column, i] == Field.empty)
            {
                grid[column, i] = Field.playertwo;

                if (LightAlgorithm(column, i, times))
                {
                    grid[column, i] = Field.empty;
                    return true;
                }
                grid[column, i] = Field.empty;
                break;
            }
        }

        return false;
    }

    public bool CheckIfLoosing(int column)
    {
        for (int i = grid.GetLength(1) - 1; i >= 0; i--)
        {
            if (grid[column, i] == Field.empty)
            {
                grid[column, i] = Field.playerone;

                if (LightAlgorithm(column, i, 3))
                {
                    grid[column, i] = Field.empty;
                    return true;
                }
                grid[column, i] = Field.empty;
                break;
            }
        }

        return false;
    }

    void declareWinner(bool p1Won)
    {
        GetComponent<GameController>().setVictory(p1Won);
    }

    //I got this algorithm off the internet
    bool LightAlgorithm(int indexX, int indexY, int times)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (grid.GetLength(0) > indexX + x && indexX + x >= 0 && grid.GetLength(1) > indexY + y && indexY + y >= 0)
                {
                    if (x != 0 || y != 0)
                    {
                        if (grid[indexX, indexY] == grid[indexX + x, indexY + y])
                        {
                            int currentPoints = 0;
                            for (int j = -3; j < 4; j++)
                            {
                                if (grid.GetLength(0) > indexX + (x * j) && indexX + (x * j) >= 0 && grid.GetLength(1) > indexY + (y * j) && indexY + (y * j) >= 0)
                                {
                                    if (grid[indexX, indexY] == grid[indexX + (x * j), indexY + (y * j)])
                                    {
                                        currentPoints++;

                                        if (currentPoints > times)
                                        {
                                            return true;
                                        }
                                    }
                                    else {
                                        currentPoints = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    int CheckBoundaryAndSetPosX()
    {
        int idx = 0;
        float minDistance = 100f;
        for (int i = 0; i < gridWidth.Length; i++)
        {
            if (curBrick.transform.position.x > gridWidth[i])
            {
                if (minDistance > curBrick.transform.position.x - gridWidth[i])
                {
                    idx = i;
                    minDistance = curBrick.transform.position.x - gridWidth[idx];
                }
            }
            else {
                if (minDistance > gridWidth[i] - curBrick.transform.position.x)
                {
                    idx = i;
                    minDistance = gridWidth[idx] - curBrick.transform.position.x;
                }
            }
        }

        curBrick.transform.position = new Vector3(gridWidth[idx], curBrick.transform.position.y, curBrick.transform.position.z);
        return idx;
    }

    void DropBrick()
    {
        curBrick.GetComponent<Rigidbody>().useGravity = true;
    }

    int DropRandomBrick()
    {
        int r = Random.Range(0, dropPositions.Count - 1);
        curBrick.transform.position = (Vector3)dropPositions[r];
        curBrick.GetComponent<Rigidbody>().useGravity = true;
        return r;
    }

    void DropBrickAtPos(int column)
    {
        curBrick.transform.position = (Vector3)dropPositions[column];
        curBrick.GetComponent<Rigidbody>().useGravity = true;
    }

    public void StartPlayerTurn(bool isP1)
    {
        isActive = true;
        isPlayerOneTurn = isP1;
        curBrick = Instantiate(isPlayerOneTurn ? p1Brick : p2Brick, new Vector3(0, height, transform.position.z),
            (isPlayerOneTurn ? p1Brick.transform.rotation : p2Brick.transform.rotation));
    }
}