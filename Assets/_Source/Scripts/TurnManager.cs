using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public enum TurnState
{
    Player1,
    Player2
}

public class TurnManager : MonoBehaviour
{
    [SerializeField] private TurnState _startingTurn;
    [SerializeField] private UnityEvent _onPlayer1TurnEvent;
    [SerializeField] private UnityEvent _onPlayer2TurnEvent;

    public bool IsMoving { get; private set; }

    public TurnState CurrentTurn
    {
        get => _currentTurn;
        set
        {
            _currentTurn = value;
            _playerTurnEvent = _currentTurn switch
            {
                TurnState.Player1 => _onPlayer1TurnEvent,
                TurnState.Player2 => _onPlayer2TurnEvent,
                _ => null
            };

            _playerTurnEvent?.Invoke();
            OnPlayerTurnChanged?.Invoke(_currentTurn);
        }
    }

    public static TurnManager Instance { get; private set; }
    public static UnityAction<TurnState> OnPlayerTurnChanged;

    private TurnState _currentTurn;
    private UnityEvent _playerTurnEvent;

    public void ChangeTurn()
    {
        CurrentTurn = CurrentTurn switch
        {
            TurnState.Player1 => TurnState.Player2,
            TurnState.Player2 => TurnState.Player1,
            _ => _startingTurn
        };
    }

    public void InvokeCurrentTurn()
    {
        CurrentTurn = _currentTurn; // Used to invoke `OnPlayerTurnChanged`
    }

    public void StartMoving()
    {
        IsMoving = true;
        UIManager.Instance.SetTurnText($"{UIManager.FormattedTurnString(_currentTurn)} is moving ...");
    }

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        await UniTask.NextFrame();
        CurrentTurn = _startingTurn;
    }
}