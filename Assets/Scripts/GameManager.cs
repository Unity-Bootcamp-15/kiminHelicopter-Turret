using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SliderController _sliderController;
    [SerializeField] private Button _createMazeButton;
    [SerializeField] private TMP_Text _mazeSizeText;

    [SerializeField] private Board _board;
    [SerializeField] private Player _player;

    private Camera _mainCamera;

    public void Start()
    {
        _sliderController.SlideValueChange += SetSlideValue;
        _createMazeButton.onClick.AddListener(CreateMaze);

        _mainCamera = Camera.main;
    }

    private void SetSlideValue(float val)
    {
        _mazeSizeText.text = val.ToString();
        _board.Size = (int)val;
    }

    private void CameraSetting()
    {
        _mainCamera.transform.position = new Vector3(_board.Size / 2, _board.Size, -_board.Size / 2);
        _mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
        _mainCamera.clearFlags = CameraClearFlags.SolidColor;
        _mainCamera.backgroundColor = Color.black;
    }

    public void CreateMaze()
    {
        CameraSetting();

        _board.Initialize();
        _board.Spawn();

        _player.Initialize(1, 1, /* 9)_board.Size - 2, _board.Size - 2,*/ _board);
    }
}
