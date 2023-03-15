using DG.Tweening;
using TMPro;
using UnityEngine;


public class MoneyCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _label;

    public Vector3 CanvasPosition => transform.position;

    private int _money;
    private int _displayedMoney;
    private Tween _gainAnimation;
    private Vector3 _initialPosition;


    public void AddMoney(int money)
    {
        if (money <= 0)
        {
            return;
        }

        _money += money;

        StartMoneyGainAnimation();
    }


    private void Awake()
    {
        _initialPosition = transform.position;
    }


    private void StartMoneyGainAnimation()
    {
        if (!NoGainAnimPlayingNow())
        {
            CancelGainAnim();
        }
        StartNewGainAnim();
    }


    private bool NoGainAnimPlayingNow() => _gainAnimation?.IsComplete() ?? false;
    
    private void CancelGainAnim()
    {
        _gainAnimation.Kill();
        _gainAnimation = null;
    }

    private void StartNewGainAnim()
    {

        var duration = GameConstants.GetInstance().moneyGainAnimDuration;
        var seq = DOTween.Sequence();
        _gainAnimation = seq
            .AppendCallback(() => ResetInitialPosition()) // ѕредотвращ€ем смещение из-за тр€ски 
            .Append(transform.DOShakePosition(duration, 20, 100))
            .Join(TextAnim(duration))
            .AppendCallback(() => ResetInitialPosition()) // ѕредотвращ€ем смещение из-за тр€ски 
            .Play();
        
    }


    private void ResetInitialPosition() => transform.position = _initialPosition;


    private Tweener TextAnim(float duration) =>
        DOTween.To(
                () => _displayedMoney,
                val => _displayedMoney = val,
                _money,
                duration)
        .OnUpdate(() => _label.text = _displayedMoney.ToString());


}
