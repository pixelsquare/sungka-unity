using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SceneLoadButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private SceneName _sceneName;

    private void Awake()
    {
        _button ??= GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClicked);

    }

    private void OnButtonClicked()
    {
        SceneManager.Instance.LoadScene(_sceneName);
    }
}
