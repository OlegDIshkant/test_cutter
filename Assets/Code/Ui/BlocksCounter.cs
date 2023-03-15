using TMPro;
using UnityEngine;


public class BlocksCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _label;
    private int _max;
    private int _current;

    public void SetMax(int max)
    {
        if (_max == max)
        {
            return;
        }
        _max = max;
        UpdateText();
    }

    public void SetCurrent(int current)
    {
        if (_current == current)
        {
            return;
        }
        _current = current;
        UpdateText();
    }


    private void UpdateText()
    {
        var text = $"{_current} / {_max}";
        _label.text = text;
    }


}
