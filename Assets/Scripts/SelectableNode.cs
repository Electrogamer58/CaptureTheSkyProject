using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectedState { Selected, Unselected, Neighbor }
public class SelectableNode : MonoBehaviour
{
    [SerializeField] Color _unselectedColor;
    [SerializeField] Color _selectedColor;
    [SerializeField] Color _neighborColor;

    public SelectedState SelectState { get; private set; }
    
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    public void ChangeSelectionState(SelectedState selectedState)
    {
        SelectState = selectedState;

        SwitchColor(SelectState);
    }
    private void SwitchColor(SelectedState selectedState)
    {
        if (selectedState == SelectedState.Selected)
        {
            if (_unselectedColor == null)
                Debug.LogError("No color");
            else
                _spriteRenderer.color = _selectedColor;
        }
        else if (selectedState == SelectedState.Unselected)
        {
            if (_unselectedColor == null)
                Debug.LogError("No color");
            else
                _spriteRenderer.color = _unselectedColor;
        }
        else if (selectedState == SelectedState.Neighbor)
        {
            if (_unselectedColor == null)
                Debug.LogError("No color");
            else
                _spriteRenderer.color = _neighborColor;
        }
    }
}
