using UnityEngine;

public class Board : MonoBehaviour
{
    public enum TileType
    {
        Empty,
        Wall,
        //4) 추가하기
        Goal
    }

    public TileType[,] Tile { get; private set; }
    public int Size { get; set; }

    // 1) 추가하기
    public int DestY { get; private set; }
    public int DestX { get; private set; }

    private Transform _maze;
    private Shader shader;
    private Material emptyMat;
    private Material wallMat;
    // 5) 추가하기
    private Material goalMat;

    private void Start()
    {
        shader = Shader.Find("Universal Render Pipeline/Lit");
        emptyMat = new Material(shader);
        emptyMat.color = Color.gray;
        wallMat = new Material(shader);
        wallMat.color = Color.white;
        // 6) 추가하기 
        goalMat = new Material(shader);
        goalMat.color = Color.green;
    }

    public void Initialize()
    {
        Tile = new TileType[Size, Size];

        // 2) 추가하기
        DestY = Size - 2;
        DestX = Size - 2;

        // 3) 없애기
        //for (int y = 0; y < Size; y++)
        //{
        //    for (int x = 0; x < Size; x++)
        //    {
        //        if (x == 0 || x == Size - 1 || y == 0 || y == Size - 1)
        //            Tile[y, x] = TileType.Wall;
        //        else
        //            Tile[y, x] = TileType.Empty;
        //    }
        //}

        GenerateBySideWineder();
    }

    public void Spawn()
    {
        if (_maze != null) 
            Despawn();

        _maze = new GameObject().transform;
        _maze.parent = transform;
        _maze.name = "Maze";

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.position = new Vector3(x, 0, -y);
                go.transform.parent = _maze;

                if (Tile[y, x] == TileType.Empty /*10) 추가하기->>>>>*/|| Tile[y, x] == TileType.Goal)
                    go.transform.localScale = new Vector3(1f, 0.1f, 1f);
                else
                    go.transform.localScale = new Vector3(1f, 2f, 1f);

                go.GetComponent<MeshRenderer>().sharedMaterial = GetTileColor(Tile[y, x]);
            }
        }
    }

    private void GenerateBySideWineder()
    {
        // 일단 길 다 막음
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                // x와 y 좌표가 짝수인경우 - 빨갛게 만듬(벽)
                if (x % 2 == 0 || y % 2 == 0)
                    Tile[y, x] = TileType.Wall;
                // 8) 추가하기
                else if (x == DestX && y == DestY)
                    Tile[y, x] = TileType.Goal;
                else
                    Tile[y, x] = TileType.Empty;
            }
        }

        // 랜덤으로 우측 혹은 아래로 길 뚫음
        for (int y = 0; y < Size; y++)
        {
            // 아래로 뚫기전 카운트 저장
            int count = 1;
            for (int x = 0; x < Size; x++)
            {
                // 아까 빨간색으로 막은 벽은 건너뜀
                if (x % 2 == 0 || y % 2 == 0)
                    continue;

                // 가장 오른쪽 끝 이고 가장 아래쪽 인 애는 건너뜀
                if (y == Size - 2 && x == Size - 2)
                    continue;

                // 가장 오른쪽 끝이면 무조건 벽풀고 길로
                if (y == Size - 2)
                {
                    Tile[y, x + 1] = TileType.Empty;
                    continue;
                }

                // 가장 아래쪽 끝이면 무조건 벽풀고 길로
                if (x == Size - 2)
                {
                    Tile[y + 1, x] = TileType.Empty;
                    continue;
                }

                // 결국 x, y 좌표가 모두 홀수인 애들을 대상으로
                // 0 과 1중 랜덤 뽑음
                // 0 이면 오른쪽 길뚫고 카운트 1증가
                if (Random.Range(0, 2) == 0)
                {
                    Tile[y, x + 1] = TileType.Empty;
                    count++;
                }
                // 1 이면 아래쪽 길뚫는데 여태까지 오른쪽으로 뚫었던 x 중 한개를 골라서 뚫음
                else
                {
                    //           x - 라는건 지금 진행이 왼쪽에서 오른쪽으로 뚫고 있었기 때문
                    //                                      * 2 해준 이유는 짝수좌표애들이 있기때문에 곱하기 2를 해줌
                    //               즉 오른쪽으로 3번 뚫은 상태라면 0, 1, 2 중에 하나 나올꺼고
                    //                                              만약 2가 나오면 곱하기 2해서 x - 4 즉 맨 왼쪽에서 아래로 길이 남
                    Tile[y + 1, x - Random.Range(0, count) * 2] = TileType.Empty;
                    // 그리고 카운트 즉 오른쪽 뚫었던 내역 초기화
                    count = 1;
                }

            }
        }

    }

    private Material GetTileColor(TileType type)
    {
        switch (type)
        {
            case TileType.Empty:
                return emptyMat;
            case TileType.Wall:
                return wallMat;
                // 7) 추가하기
            case TileType.Goal:
                return goalMat;
        }

        return wallMat;
    } 

    private void Despawn()
    {
        Destroy(_maze.gameObject);
        _maze = null;
    }
}
