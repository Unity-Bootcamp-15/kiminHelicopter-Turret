using System.Collections.Generic;
using UnityEngine;

// 42) 추가하기
class Pos
{
    public Pos(int y, int x) { Y = y; X = x; }

    public int Y;
    public int X;
}

public class Player : MonoBehaviour
{
    public int PosY { get; private set; }
    public int PosX { get; private set; }

    private Board _board;
    private bool _isBoardCreated = false;

    // 28) 추가하기
    enum Dir
    {
        Up = 0, // (0 - 1 + 4 = 3) % 4 = 3 
        Left = 1,
        Down = 2,
        Right = 3
    }

    // 4, 3
    //    [ ]
    // [ ]   [ ]
    //    [P] 

    // 29) 추가하기
    int _dir = (int)Dir.Up;

    // 43) 추가하기
    List<Pos> _points = new List<Pos>();

    public void Initialize(int posY, int posX, /* 9 삭제하기 int destY, int dextX,*/ Board board)
    {
        _isBoardCreated = true;

        PosY = posY;
        PosX = posX;
        _board = board;

        transform.position = new Vector3(posX, 0, -posY);

        RightHand();        
    }

    void BFS()
    {
        int[] deltaY = new int[] { -1, 0, 1, 0 };
        int[] deltaX = new int[] { 0, -1, 0, 1 };

        bool[,] found = new bool[_board.Size, _board.Size];
        Pos[,] parent = new Pos[_board.Size, _board.Size];

        Queue<Pos> queue = new Queue<Pos>();
        queue.Enqueue(new Pos(PosY, PosX));
        found[PosY, PosX] = true;
        parent[PosY, PosX] = new Pos(PosY, PosX);

        while (queue.Count > 0)
        {
            Pos pos = queue.Dequeue();
            int nowY = pos.Y;
            int nowX = pos.X;

            for (int i = 0; i < 4; i++) 
            {
                int nextY = nowY + deltaY[i];
                int nextX = nowX + deltaX[i];

                if (nextX < 0 || nextX >= _board.Size || nextY < 0 || nextY >= _board.Size)
                    continue;
                if (_board.Tile[nextY, nextX] == Board.TileType.Wall)
                    continue;
                if (found[nextY, nextX] == true)
                    continue;

                queue.Enqueue(new Pos(nextY, nextX));
                found[nextY, nextX] = true;
                parent[nextY, nextX] = new Pos(nowY, nowX);
            }
        }

        int y = _board.DestY;
        int x = _board.DestX;

        while (parent[y, x].Y != y || parent[y, x].X != x)
        {
            _points.Add(new Pos(y, x));
            Pos pos = parent[y, x];
            y = pos.Y;
            x = pos.X;
        }

        _points.Add(new Pos(y, x));
        _points.Reverse();
    }

    void RightHand() // 구현/시뮬레이션 알고리즘에 단골 소재 (게임쪽)
    {
        // 36) 추가하기
        int[] _frontY = new int[] { -1, 0, 1, 0 };
        int[] _frontX = new int[] { 0, -1, 0, 1 };

        // 40) 추가하기
        int[] _rightY = new int[] { 0, -1, 0, 1 };
        int[] _rightX = new int[] { 1, 0, -1, 0 };

        // 44) 추가하기
        _points.Add(new Pos(PosY, PosX));

        // 21) 추가하기
        // 목적지 계산전까지 계속실행
        while (PosY != _board.DestY || PosX != _board.DestX)
        {
            // 22) 추가하기
            // 1. 현재 바라보는 방향을 기준으로 오른쪽으로 갈수 있는지 확인

            // 23) 추가하기
            if (/* 41) 추가하기->>>*/_board.Tile[PosY + _rightY[_dir], PosX + _rightX[_dir]] != Board.TileType.Wall)
            {
                // 오른쪽 방향으로 90도 회전
                // 34) 추가하기
                _dir = (_dir - 1 + 4) % 4;


                // 33) 삭제하기
                //// 30) 추가하기
                //switch (_dir)
                //{
                //    // 31) 추가하기
                //    case (int)Dir.Up:
                //        _dir = (int)Dir.Right; // 32) 추가하기
                //        break;
                //    case (int)Dir.Left:
                //        break;
                //}
                // 앞으로 한보 전진
                // 37) 추가하기
                PosY = PosY + _frontY[_dir];
                PosX = PosX + _frontX[_dir];
                // 45) 추가하기
                _points.Add(new Pos(PosY, PosX));
            }

            // 24) 추가하기
            // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인

            // 25) 추가하기
            else if (/* 39) 추가하기->>>*/_board.Tile[PosY + _frontY[_dir], PosX + _frontX[_dir]] != Board.TileType.Wall)
            {
                // 앞으로 한보 전진
                // 38) 추가하기
                PosY = PosY + _frontY[_dir];
                PosX = PosX + _frontX[_dir];
                // 45) 추가하기
                _points.Add(new Pos(PosY, PosX));
            }

            // 26) 추가하기
            // 3. 내 오른쪽, 내앞 모두 벽이 있다면
            else
            {
                // 27) 추가하기
                // 왼쪽 방향으로 90도 회전 후 턴 넘기기

                // 35) 추가하기
                _dir = (_dir + 1 + 4) % 4;
            }
        }
    }

    const float MOVE_TICK = 0.1f;
    float _sumTick = 0;
    // 46) 추가하기
    int _lastIndex = 0;
    // 11) 변경하기
    private void Update()
    {
        // 49) 추가하기
        if (_lastIndex >= _points.Count)
            return;

        // 12) 위치 수정하기
        if (_isBoardCreated == false)
            return;

        _sumTick += Time.deltaTime;
        // 13) 수정하기
        if (_sumTick < MOVE_TICK)
            return;

        _sumTick = 0;

        // 47) 삭제하기
        //// 0.1 초마다 실행될 로직
        //int /*14) 수정하기->>>>*/dir = Random.Range(0, 5);

        //// 15) 추가하기
        //int nextY = PosY;
        //int nextX = PosX;

        //switch (dir)
        //{
        //    case 0:
        //        // 16) 수정하기
        //        nextY = PosY - 1;
        //        break;
        //    case 1:
        //        nextY = PosY + 1;
        //        break;
        //    case 2:
        //        nextX = PosX - 1;
        //        break;
        //    case 3:
        //        nextX = PosX + 1;
        //        break;
        //}

        //// 17) 추가하기 
        //if (nextY < 0 || nextY >= _board.Size) return;
        //if (nextX < 0 || nextX >= _board.Size) return;
        //if (_board.Tile[nextY, nextX] != Board.TileType.Empty) return;

        //// 18) 추가하기
        //PosY = nextY;
        //PosX = nextX;
        //transform.position = new Vector3(PosX, 0, -PosY);

        // 48) 수정하기
        PosY = _points[_lastIndex].Y;
        PosX = _points[_lastIndex].X;
        _lastIndex++;
        transform.position = new Vector3(PosX, 0, -PosY);

        // 19) 추가하기
        //if (PosY - 1 >= 0 && _board.Tile[PosY - 1, PosX] == Board.TileType.Empty) 
        //{ 
        //    transform.position = new Vector3(PosX, 0, -PosY + 1); PosY--; 
        //}
        //break;
    }

    // 20) 추가하기
    public void Test(int dir)
    {
        int nextY = PosY;
        int nextX = PosX;

        switch (dir)
        {
            case 0:
                nextY = PosY - 1;
                break;
            case 1:
                nextY = PosY + 1;
                break;
            case 2:
                nextX = PosX - 1;
                break;
            case 3:
                nextX = PosX + 1;
                break;
        }

        if (nextY < 0 || nextY >= _board.Size) return;
        if (nextX < 0 || nextX >= _board.Size) return;
        if (_board.Tile[nextY, nextX] != Board.TileType.Empty) return;

        PosY = nextY;
        PosX = nextX;
        transform.position = new Vector3(PosX, 0, -PosY);
    }
}
