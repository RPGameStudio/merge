using System;
using System.Threading.Tasks;
using Data;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Gameplay
{
    public struct CoinViewInitialData
    {
        public CoinState State;
        public Action<CoinState> OnCollect;
    }

    public class CoinView : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        public CoinViewInitialData Data { get; private set; }

        [Inject(Id = "main_tracker")] private BalanceTracker _tracker;
        [Inject] private InputService _inputService;

        public void Initialize(CoinViewInitialData initialData) => Data = initialData;

        public async void OnPointerEnter(PointerEventData eventData)
        {
            if (Input.GetMouseButton(0) && !_inputService.IsDraggingEntity)
            {
                _ = Collect();
            }
        }

        private async Task DestroyAnimation()
        {
            var from = transform.position;
            var to = _tracker.transform.position;
            var middle = from + (to - from) / 2;
            var randomPoint = UnityEngine.Random.value > 0.5f ? new Vector3(-middle.y, middle.x) : new Vector3(middle.y, -middle.x);

            await transform.DOPath(new Vector3[] { transform.position, middle + randomPoint / 8, _tracker.transform.position },
                                   0.5f,
                                   PathType.CatmullRom,
                                   PathMode.Sidescroller2D,
                                   10,
                                   Color.red)
                           .OnComplete(() => Destroy(gameObject)).AsyncWaitForCompletion();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _ = Collect();
        }

        private async Task Collect()
        {
            enabled = false;
            Data.OnCollect?.Invoke(Data.State);
            await DestroyAnimation();
            _tracker.AddBalanceVisual(Data.State.Profit);
        }
    }
}