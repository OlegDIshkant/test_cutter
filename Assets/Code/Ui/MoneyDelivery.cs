using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;


public class MoneyDelivery
{
    public struct Result
    {
        public int moneyToAdd;
    }

    private class DeliveryInfo
    {
        public RectTransform coin;
        public Tween tween;
    }


    private readonly List<DeliveryInfo> _deliveries = new List<DeliveryInfo>();
    private readonly List<DeliveryInfo> _finishedDeliveries = new List<DeliveryInfo>();
    private readonly Vector3 _uiDelivaryTarget;
    private readonly Func<string, Vector2, RectTransform> _SpawnPrefabOnCanvas;


    public MoneyDelivery(Func<string, Vector2, RectTransform> SpawnPrefabOnCanvas, Vector3 uiDelivaryTarget)
    {
        _uiDelivaryTarget = uiDelivaryTarget;
        _SpawnPrefabOnCanvas = SpawnPrefabOnCanvas;
    }


    public Result Update(InputInfo input)
    {
        StartmentOfCoinDeliveries(input);
        FinishmentOfCoinDeliviries(out var moneyToAdd);
        return new Result() { moneyToAdd = moneyToAdd };
    }


    private void StartmentOfCoinDeliveries(InputInfo input)
    {
        for (int i = 0; i < input.blockSoldThisFrame; i++)
        {
            StartDelivery(input.blocksVanishPoint);
        }
    }


    private void StartDelivery(Vector3 blocksVanishPoint)
    {
        var coin = SpawnCoin(blocksVanishPoint);
        var delivery = new DeliveryInfo() { coin = coin, tween = PrepareTweenForCoin(coin) };
        _deliveries.Add(delivery);

        delivery.tween
            .OnComplete(() => _finishedDeliveries.Add(delivery))
            .Play();
    }


    private RectTransform SpawnCoin(Vector3 blocksVanishPoint)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(blocksVanishPoint);
        Vector3 uiPos = new Vector2(screenPos.x, /*Screen.height - */screenPos.y);
        return _SpawnPrefabOnCanvas(GameConstants.GetInstance().pathToCoinPrefab, uiPos);
    }


    private Tween PrepareTweenForCoin(RectTransform coin)
    {
        var duration = GameConstants.GetInstance().coinDeliveryDuration;
        var scaleFactor = 2f;
        var higherScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        var delay = GameConstants.GetInstance().coinDeliveryDelay;

        var seq = DOTween.Sequence();
        seq
            .AppendCallback(() => coin.gameObject.SetActive(false))
            .AppendInterval(delay)
            .AppendCallback(() => coin.gameObject.SetActive(true))
            .Append(coin.DOMove(_uiDelivaryTarget, duration))
            .Join(coin.DOScale(higherScale, duration / 2).SetLoops(2, LoopType.Yoyo))
            .AppendCallback(() => coin.gameObject.SetActive(false));

        return seq;
    }


    private void FinishmentOfCoinDeliviries(out int moneyToAdd)
    {
        moneyToAdd = 0; 

        foreach (var delivery in _finishedDeliveries)
        {
            _deliveries.Remove(delivery);
            GameObject.Destroy(delivery.coin.gameObject);
            moneyToAdd += GameConstants.GetInstance().moneyPerCoin;
        }

        _finishedDeliveries.Clear();
    }

}
