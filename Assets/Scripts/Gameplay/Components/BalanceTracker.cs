using System.Collections;
using System.Collections.Generic;
using Data;
using TMPro;
using UnityEngine;
using Zenject;
using RX;
using System.Threading.Tasks;

namespace UI
{
    public class BalanceTracker : MonoBehaviour
    {
        [SerializeField] private bool _trackDataChanges = true;
        [SerializeField] private TextMeshProUGUI _label;

        [Inject] private UserState _userState;

        private int _currentBalanceVisual;

        private void Awake()
        {
            if (_trackDataChanges)
            {
                _userState.Balance.Subscribe(UpdateBalanceView).AddTo(this);
            }
            else
            {
                _ = UpdateBalanceView(_userState.Balance.Value);
            }

        }

        public void SetBalanceVisual(int value)
        {
            _ = UpdateBalanceView(value);
        }


        public void AddBalanceVisual(int value)
        {
            _ = UpdateBalanceView(_currentBalanceVisual + value);
        }


        private async Task UpdateBalanceView(int x)
        {
            _currentBalanceVisual = x;
            _label.text = _currentBalanceVisual.ToString();
        }
    }
}