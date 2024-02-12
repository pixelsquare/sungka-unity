using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Pot : MonoBehaviour
{
    [SerializeField] private bool _interactable = true;
    [SerializeField] private Outline _outline;
    [SerializeField] private TMP_Text _beadsLabel;

    public int Index => _index;
    public int BeadCount => _beadList.Count;
    public bool IsEmpty => _beadList.Count == 0;

    public bool Interactable
    {
        get => _interactable;
        set => _interactable = value;
    }

    private bool IsSelectable => _interactable && !IsEmpty;

    private int _index = -1;
    private int _beadCount = 0;
    private UnityAction<Pot> _onPotSelected;
    private List<GameObject> _beadList = new();

    public void Initialize(int index, int beadCount, bool interactable = true, UnityAction<Pot> onPotSelected = null)
    {
        _index = index;
        _beadCount = beadCount;
        _interactable = interactable;
        _onPotSelected = onPotSelected;
        AddBeads(_beadCount);
    }

    public void AddBeads(int count = 1)
    {
        for (var i = 0; i < count; i++)
        {
            var bead = BeadsPoolManager.Instance.GetBeadObject();
            bead.transform.position = transform.position + (Vector3.up * 0.5f + Random.insideUnitSphere * .35f);
            bead.SetActive(true);
            _beadList.Add(bead);
        }

        SetBeadsText($"{BeadCount}");
    }

    public void RemoveBeads(int count = 1)
    {
        _beadList.Where(x => x.activeInHierarchy)
                 .Take(count)
                 .ToList()
                 .ForEach(x => 
                 {
                     x.SetActive(false);
                     _beadList.Remove(x);
                 });

        SetBeadsText($"{BeadCount}");
    }

    public void ClearBeads(bool hideAll = true)
    {
        if (hideAll)
        {
            _beadList.Where(x => x.activeInHierarchy)
                    .ToList()
                    .ForEach(x => x.SetActive(false));
        }
                 
        _beadList?.Clear();
        SetBeadsText($"{BeadCount}");
    }

    private void SetOutlineActive(bool active)
    {
        _outline.enabled = active;
    }

    private void SetBeadsText(string text)
    {
        if (_beadsLabel == null)
        {
            return;
        }

        _beadsLabel.text = text;
    }

    private void OnMouseEnter()
    {
        if (!IsSelectable)
        {
            return;
        }

        SetOutlineActive(true);
    }

    private void OnMouseExit()
    {        
        if (!IsSelectable)
        {
            return;
        }

        SetOutlineActive(false);
    }

    private void OnMouseDown()
    {
        if (!IsSelectable)
        {
            return;
        }

        SetOutlineActive(false);
        _onPotSelected?.Invoke(this);
    }
}
