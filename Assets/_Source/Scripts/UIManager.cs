using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _turnLabel;
    [SerializeField] private GameObject _exitPanel;

    public static UIManager Instance { get; private set; }

    public static string FormattedTurnString(TurnState turn)
    {
        return turn switch
        {
            TurnState.Player1 => "<color=#0000FF>Player 1</color>",
            TurnState.Player2 => "<color=#FF0000>Player 2</color>",
            _ => "" 
        };
    }

    public void SetTurnText(string text)
    {
        _turnLabel.text = text;
    }

    public void SetExitButtonActive(bool active)
    {
        _exitPanel.SetActive(active);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        TurnManager.OnPlayerTurnChanged += OnTurnChanged;
    }

    private void OnDisable()
    {
        TurnManager.OnPlayerTurnChanged -= OnTurnChanged;
    }

    private void Start()
    {
        SetExitButtonActive(false);
    }

    private void OnTurnChanged(TurnState turnState)
    {
        SetTurnText($"Current Turn\n{FormattedTurnString(turnState)}");
    }
}
