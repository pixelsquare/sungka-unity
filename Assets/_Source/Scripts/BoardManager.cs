using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private int _beadCount = 7;
    [SerializeField] private Pot _p1Pot;
    [SerializeField] private Pot _p2Pot;
    [SerializeField] private List<Pot> _pots;

    public static BoardManager Instance { get; private set; }
    private const int PlayerPotSize = 7;

    private Dictionary<int, int> _potPairsMap = new();
    private Dictionary<TurnState, Pot> _mainPotMap = new();
    private Dictionary<TurnState, List<Pot>> _playerPotsMap = new();

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        TurnManager.OnPlayerTurnChanged += OnPlayerTurnChanged;
    }

    private void OnDisable()
    {
        TurnManager.OnPlayerTurnChanged -= OnPlayerTurnChanged;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        for (var i = 0; i < _pots.Count; i++)
        {
            var beadCount = _beadCount;

            // Bypass home pots on both players.
            if (_pots[i] == _p1Pot || _pots[i] == _p2Pot)
            {
                beadCount = 0;
            }

            _pots[i].Initialize(i, beadCount, false, OnPotSelected);
        }

        for (var i = 0; i < PlayerPotSize; i++)
        {
            _potPairsMap.Add(i, _pots.Count - i - 2);
            _potPairsMap.Add(_pots.Count - i - 2, i);
        }

        _mainPotMap.Add(TurnState.Player1, _p1Pot);
        _mainPotMap.Add(TurnState.Player2, _p2Pot);

        _playerPotsMap.Add(TurnState.Player1, _pots.Take(PlayerPotSize).ToList());
        _playerPotsMap.Add(TurnState.Player2, _pots.Skip(PlayerPotSize + 1).Take(PlayerPotSize).ToList());
    }

    // Collect opponent's adjacent pot when the last bead lands to an empty pot on the player's side.
    private void GatherOpponentBeads(Pot lastPot)
    {
        var curTurn = TurnManager.Instance.CurrentTurn;

        if (lastPot.BeadCount > 1
            || !_playerPotsMap[curTurn].Contains(lastPot)
            || !_potPairsMap.ContainsKey(lastPot.Index))
        {
            return;
        }

        var otherIdx = _potPairsMap[lastPot.Index];
        var otherPot = _pots[otherIdx];

        // Do nothing if there's no bead to gather.
        if (otherPot.IsEmpty)
        {
            return;
        }

        var curPot = _mainPotMap[curTurn];
        curPot.AddBeads(otherPot.BeadCount + lastPot.BeadCount);

        otherPot.ClearBeads();
        lastPot.ClearBeads();
    }

    private void EndTurn(Pot lastPot)
    {
        // Additional turn if the last bead lands on the player's home.
        var curTurn = TurnManager.Instance.CurrentTurn;

        if (lastPot != null && lastPot == _mainPotMap[curTurn])
        {
            TurnManager.Instance.InvokeCurrentTurn();
            return;
        }

        TurnManager.Instance.ChangeTurn();
    }

    private void SetPlayerPotsInteractable(TurnState playerTurn, bool interactable)
    {
        var curPlayerPots = _playerPotsMap[playerTurn];
        curPlayerPots.ForEach(x => x.Interactable = interactable);
    }

    private async void OnPotSelected(Pot pot)
    {
        TurnManager.Instance.StartMoving();
        
        var curTurn = TurnManager.Instance.CurrentTurn;
        SetPlayerPotsInteractable(curTurn, false);

        var potSize = pot.BeadCount;
        pot.RemoveBeads(potSize);

        var startIdx = pot.Index;
        var lastPot = _pots[startIdx];

        for (var i = 0; i < potSize; i++)
        {
            startIdx++;

            var idx = startIdx % _pots.Count;
            lastPot = _pots[idx];
            lastPot.AddBeads();

            await UniTask.Delay(500);
        }

        GatherOpponentBeads(lastPot);
        EndTurn(lastPot);
    }

    private void OnPlayerTurnChanged(TurnState playerTurn)
    {
        if (_playerPotsMap.Values.All(a => a.All(b => b.IsEmpty)))
        {
            Debug.Log("Game End!");
            UIManager.Instance.SetExitButtonActive(true);

            // Check for draw.
            var isDraw = _mainPotMap.Values.All(x => x.BeadCount == _mainPotMap.Values.First().BeadCount);

            if (isDraw)
            {
                UIManager.Instance.SetTurnText($"DRAW!");
                return;
            }

            // Compare home pots and get the highest bead count.
            var winnerList = from entry in _mainPotMap 
                             orderby entry.Value.BeadCount descending 
                             select entry.Key;

            var winner = winnerList.FirstOrDefault();
            UIManager.Instance.SetTurnText($"{UIManager.FormattedTurnString(winner)}\nWins!");
            return;
        }

        // Immediately end the player's turn if there are no beads on its side.
        if (_playerPotsMap[playerTurn].All(x => x.IsEmpty))
        {
            EndTurn(null);
            return;
        }

        SetPlayerPotsInteractable(playerTurn, true);
    }
}
